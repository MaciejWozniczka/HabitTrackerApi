namespace HabitTrackerApi.Habits;

[ApiController]
public class DeleteHabit(IMediator mediator) : ControllerBase
{
    [Authorize]
    [SwaggerOperation(Tags = ["Habits"])]
    [HttpDelete("/api/habit/{id}")]
    public async Task<Result> DeleteHabitAsync(Guid id)
    {
        return await mediator.Send(new DeleteHabitCommand(id));
    }

    public class DeleteHabitCommand(Guid id) : IRequest<Result>
    {
        public Guid Id { get; set; } = id;
    }

    public class DeleteHabitCommandHandler(IHabitService habitService) : IRequestHandler<DeleteHabitCommand, Result>
    {
        public async Task<Result> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
        {
            await habitService.Delete(request.Id, cancellationToken);

            return Result.Ok();
        }
    }
}