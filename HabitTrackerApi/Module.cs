namespace HabitTrackerApi;

public interface IModule
{
    Task Run(IServiceProvider serviceProvider);
}
public class Module : IModule
{
    public async Task Run(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
    }
}