namespace HabitTrackerApi.Habits;

public interface IHabitService
{
    Task<List<Habit>> GetAllForUser(string userId, CancellationToken cancellationToken);
    Task<Habit> GetById(Guid id, CancellationToken cancellationToken);
    Task<HabitCheck> GetHabitCheckById(Guid id, DateTime date, CancellationToken cancellationToken);
    Task<Habit> Create(Habit model, CancellationToken cancellationToken);
    Task<HabitCheck> CreateHabitCheck(HabitCheck model, CancellationToken cancellationToken);
    Task<Habit> Update(Habit model, CancellationToken cancellationToken);
    Task<Guid> Delete(Guid id, CancellationToken cancellationToken);
    Task DeleteHabitCheck(Guid id, CancellationToken cancellationToken);
}

public class HabitService(DataContext db) : IHabitService
{
    public async Task<List<Habit>> GetAllForUser(string userId, CancellationToken cancellationToken)
    {
        return await db.Habits
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .Include(h => h.HabitCheck)
            .ToListAsync(cancellationToken);
    }

    public async Task<Habit> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await db.Habits
            .Where(h => h.Id == id && !h.IsDeleted)
            .Include(h => h.HabitCheck)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<HabitCheck> GetHabitCheckById(Guid id, DateTime date, CancellationToken cancellationToken)
    {
        return await db.HabitChecks
            .Where(h => h.HabitId == id && h.Date.Date == date.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Habit> Create(Habit model, CancellationToken cancellationToken)
    {
        await db.Habits.AddAsync(model, cancellationToken);
        await SaveChanges(cancellationToken);

        return model;
    }
    public async Task<HabitCheck> CreateHabitCheck(HabitCheck model, CancellationToken cancellationToken)
    {
        await db.HabitChecks.AddAsync(model, cancellationToken);
        await SaveChanges(cancellationToken);

        return model;
    }

    public async Task<Habit> Update(Habit model, CancellationToken cancellationToken)
    {
        model.UpdateDate = DateTimeOffset.UtcNow;
        db.Habits.Update(model);
        await SaveChanges(cancellationToken);

        return model;
    }

    public async Task<Guid> Delete(Guid id, CancellationToken cancellationToken)
    {
        var model = await db.Habits
            .Where(h => h.Id == id && !h.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        model.IsDeleted = true;
        db.Habits.Update(model);
        await SaveChanges(cancellationToken);

        return model.Id;
    }

    public async Task DeleteHabitCheck(Guid id, CancellationToken cancellationToken)
    {
        var model = await db.HabitChecks
            .Where(h => h.Id == id && !h.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        db.HabitChecks.Remove(model);
        await SaveChanges(cancellationToken);
    }

    private async Task SaveChanges(CancellationToken cancellationToken)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}