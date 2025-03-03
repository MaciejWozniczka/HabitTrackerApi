namespace HabitTrackerApi.Habits;

[ApiController]
public class AddHabit(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Habits"])]
    [HttpPost("/api/habit/")]
    public async Task<Result<Guid>> AddHabitAsync(AddHabitDto habit)
    {
        return await mediator.Send(new AddHabitCommand(habit));
    }

    public class AddHabitCommand(AddHabitDto habit) : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public AddHabitDto Habit { get; set; } = habit;
    }

    public class AddHabitDto
    {
        public string Name { get; set; }
    }

    public class AddHabitHandler(IHabitService service, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<AddHabitCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddHabitCommand request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            var habit = new Habit
            {
                Name = request.Habit.Name,
                UserId = user.Id
            };

            var result = await service.Create(habit, cancellationToken);

            return Result.Ok(result.Id);
        }
    }
}