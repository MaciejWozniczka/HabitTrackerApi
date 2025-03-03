namespace HabitTrackerApi.Habits;

public class Habit : BaseModel
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public List<HabitCheck>? HabitCheck { get; set; }
}