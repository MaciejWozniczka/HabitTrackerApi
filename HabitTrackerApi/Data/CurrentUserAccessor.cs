namespace HabitTrackerApi.Data;

public interface ICurrentUserAccessor
{
    string? GetUserEmail();
    Task<User?> GetCurrentUser();
    string? GetCurrentToken();
}

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, DataContext db) : ICurrentUserAccessor
{
    public string? GetUserEmail()
    {
        var emailClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        return emailClaim?.Value;
    }

    public async Task<User?> GetCurrentUser()
    {
        var emailClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (emailClaim == null) return null;

        return await db.Users.Where(u => u.NormalizedEmail == emailClaim.Value.ToUpper())
            .FirstOrDefaultAsync();
    }

    public string? GetCurrentToken()
    {
        var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        if (authorizationHeader.StartsWith("Bearer "))
        {
            var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            return accessToken;
        }
        else
        {
            return null;
        }
    }
}