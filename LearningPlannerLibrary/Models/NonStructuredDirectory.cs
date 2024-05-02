namespace LearningPlannerLibrary.Models;

public sealed class NonStructuredDirectory : BaseModel
{
    public NonStructuredDirectory(string name, TimeSpan? duration = null)
        : base(name, duration)
    {
        
    }

    public List<Video> Videos { get; set; } = new();
    public List<NonStructuredDirectory> SubDirectories { get; set; } = new();
    public TimeSpan TotalTimeSpan { get; set; }
    
    public TimeSpan GetDuration() => new TimeSpan(SubDirectories.Select(v => v.Duration).Sum(v => v.Ticks) + Videos.Select(v => v.Duration).Sum(v => v.Ticks));
    
    public void AddSubDirectory(NonStructuredDirectory subDirectory) => SubDirectories.Add(subDirectory);
    public void AddSubDirectory(string name, TimeSpan? duration) => SubDirectories.Add(new NonStructuredDirectory(name, duration));
    
    
    public void AddVideo(Video video) => Videos.Add(video);
    public void AddVideo(string name, TimeSpan? duration) => Videos.Add(new Video(name, duration));
}