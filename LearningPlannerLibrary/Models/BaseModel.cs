namespace LearningPlannerLibrary.Models;

public abstract class BaseModel
{
    protected BaseModel(string name, TimeSpan? duration)
    {
        Name = name;
        Duration = duration ?? new(0);
    }

    public string Name { get; set; }
    public TimeSpan Duration { get; set; }
}