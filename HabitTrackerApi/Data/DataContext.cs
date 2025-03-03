namespace HabitTrackerApi.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Habit?> Habits { get; set; }
    public DbSet<HabitCheck> HabitChecks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}