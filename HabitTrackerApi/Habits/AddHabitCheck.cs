namespace HabitTrackerApi.Habits;

[ApiController]
public class AddHabitCheck(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Habits"])]
    [HttpPost("/api/habit/{id}/check")]
    public async Task<Result<Guid>> AddHabitCheckAsync(Guid id, [FromBody] AddHabitCheckCommand command)
    {
        return await mediator.Send(command.Set(c => c.Id = id));
    }

    public class AddHabitCheckCommand : IRequest<Result<Guid>>
    {
        public AddHabitCheckCommand(Guid id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        [JsonIgnore]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class AddHabitCheckHandler(IHabitService service, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<AddHabitCheckCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddHabitCheckCommand request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            var habitCheck = new HabitCheck
            {
                Date = request.Date,
                IsDone = true,
                HabitId = request.Id
            };

            var result = await service.CreateHabitCheck(habitCheck, cancellationToken);

            return Result.Ok(result.Id);
        }
    }
}