namespace HabitTrackerApi.Habits;

public class HabitCheck : BaseModel
{
    public bool IsDone { get; set; }
    public DateTime Date { get; set; }
    public Guid HabitId { get; set; }
    public Habit Habit { get; set; }
}