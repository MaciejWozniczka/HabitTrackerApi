namespace HabitTrackerApi.Users;

[ApiController]
public class ManageUser(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Users"], Summary = "Change user")]
    [HttpPut("/api/user/{id}")]
    public async Task<Result<Guid>> ManageUserAsync(Guid id, ManageUserCommand command)
    {
        return await mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageUserCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Nationality { get; set; }
        public SexType? Sex { get; set; }
        public string? Picture { get; set; }
        public string? Description { get; set; }
    }

    public class ManageUserCommandHandler(DataContext db, ILogger<ManageUserCommandHandler> logger)
        : IRequestHandler<ManageUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ManageUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.Id.ToString();

            var user = await db.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result.NotFound<Guid>(userId);
            }

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.Nationality != null) user.Nationality = request.Nationality;
            if (request.Sex != null) user.Sex = request.Sex;
            if (request.Picture != null) user.Picture = request.Picture;
            if (request.Description != null) user.Description = request.Description;

            logger.LogInformation($"[User: {user.Id}] Updating user");

            db.Update(user);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Ok(Guid.Parse(user.Id));
        }
    }
}