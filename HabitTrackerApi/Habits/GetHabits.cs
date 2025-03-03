namespace HabitTrackerApi.Habits;

[ApiController]
public class GetHabits(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Habits"])]
    [HttpGet("/api/user/habits")]
    public async Task<Result<GetUserHabitsDto>> GetHabitsAsync()
    {
        return await mediator.Send(new GetHabitsQuery());
    }

    public class GetHabitsQuery : IRequest<Result<GetUserHabitsDto>>;

    public class GetUserHabitsDto
    {
        public List<GetHabitsDto> Habits { get; set; }
    }

    public class GetHabitsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<GetHabitChecksDto> HabitCheck { get; set; }
    }

    public class GetHabitChecksDto
    {
        public bool IsDone { get; set; }
        public DateTime Date { get; set; }
    }

    public class GetHabitsDtoQueryHandler(ICurrentUserAccessor currentUserAccessor, IHabitService habitService) : IRequestHandler<GetHabitsQuery, Result<GetUserHabitsDto>>
    {
        public async Task<Result<GetUserHabitsDto>> Handle(GetHabitsQuery request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            if (user == null)
            {
                return Result.NotFound<GetUserHabitsDto>("User not found");
            }

            var userHabits = await habitService.GetAllForUser(user.Id, cancellationToken);

            var result = new GetUserHabitsDto
            {
                Habits = userHabits
                    .Select(h => new GetHabitsDto
                    {
                        Id = h.Id,
                        Name = h.Name,
                        HabitCheck = h.HabitCheck
                            .Select(hc => new GetHabitChecksDto
                            {
                                IsDone = hc.IsDone,
                                Date = hc.Date
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return Result.Ok(result);
        }
    }
}