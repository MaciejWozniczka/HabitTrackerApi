namespace HabitTrackerApi.Users;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? Nationality { get; set; }
    public SexType? Sex { get; set; }
    public string? Picture { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public bool IsDeleted { get; set; }
    public string? RefreshToken { get; set; }
    public List<Habit> Habits { get; set; }
}
public enum SexType
{
    Male,
    Female,
    Other
}