namespace HabitTrackerApi.Data;

public class BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreateDate { get; set; } = DateTime.UtcNow;
    public DateTimeOffset UpdateDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}