namespace HabitTrackerApi.Users;

[ApiController]
public class GetCurrentUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Get current user")]
    [HttpGet("/api/user/")]
    public async Task<Result<GetCurrentUserDto>> GetCurrentUserAsync()
    {
        return await mediator.Send(new GetCurrentUserQuery());
    }

    public class GetCurrentUserQuery : IRequest<Result<GetCurrentUserDto>>
    {
    }

    public class GetCurrentUserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class GetCurrentUserQueryHandler(ICurrentUserAccessor currentUserAccessor)
        : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserDto>>
    {
        public async Task<Result<GetCurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<GetCurrentUserDto>("User not found");
            }

            var result = new GetCurrentUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Nationality = new CultureInfo(user.Nationality ?? "").NativeName,
                Sex = user.Sex,
                Picture = user.Picture,
                Description = user.Description,
                CreateDate = user.CreateDate
            };

            return Result.Ok(result);
        }
    }
}