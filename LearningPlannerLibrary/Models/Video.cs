
namespace LearningPlannerLibrary.Models;

public sealed class Video : BaseModel
{
    public Video(string name, TimeSpan? duration = null)
        : base(name, duration)
    {
        
    }
}