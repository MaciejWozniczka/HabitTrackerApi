namespace HabitTrackerApi.Data;

public class UnitOfWorkBehavior<TRequest, TResponse, TContext>(TContext db) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TContext : DbContext
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();

        if (request.GetType().Name.EndsWith("Command"))
        {
            var result = response as Result;

            if (result == null || result.Success)
            {
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        return response;
    }
}