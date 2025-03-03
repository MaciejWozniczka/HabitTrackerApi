namespace HabitTrackerApi.Habits;

[ApiController]
public class DeleteHabitCheck(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Habits"])]
    [HttpDelete("/api/habit/{id}/check")]
    public async Task<Result<Guid>> DeleteHabitCheckAsync(Guid id, [FromBody] DeleteHabitCheckCommand command)
    {
        return await mediator.Send(command.Set(c => c.Id = id));
    }

    public class DeleteHabitCheckCommand : IRequest<Result<Guid>>
    {
        public DeleteHabitCheckCommand(Guid id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        [JsonIgnore]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class DeleteHabitCheckHandler(IHabitService service, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<DeleteHabitCheckCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteHabitCheckCommand request, CancellationToken cancellationToken)
        {
            var user = await currentUserAccessor.GetCurrentUser();

            var habitCheck = await service.GetHabitCheckById(request.Id, request.Date, cancellationToken);

            await service.DeleteHabitCheck(habitCheck.Id, cancellationToken);

            return Result.Ok(habitCheck.Id);
        }
    }
}