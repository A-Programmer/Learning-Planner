namespace LearningPlannerLibrary.Models;

public sealed class Module : BaseModel
{
    public Module(string name, TimeSpan? duration = null)
        : base(name, duration)
    {
    }

    public List<Video> Videos { get; set; } = new();

    public TimeSpan GetDuration() => new TimeSpan(Videos.Select(v => v.Duration).Sum(v => v.Ticks));

    public void AddVideo(Video video) => Videos.Add(video);
    public void AddVideo(string name, TimeSpan? duration) => Videos.Add(new Video(name, duration));
    public void AddRange(IEnumerable<Video> videos) => Videos.AddRange(videos);

    public void Sort() => Videos = Videos.OrderBy(v => v.Name).ToList();
}