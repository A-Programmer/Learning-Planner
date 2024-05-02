namespace LearningPlannerLibrary.Models;

public sealed class LearningPath : BaseModel
{
    public LearningPath(string name, TimeSpan? duration = null)
        : base(name, duration)
    {
        
    }

    public List<Course> Courses { get; set; } = new();
    
    public TimeSpan GetDuration() => new TimeSpan(Courses.Select(v => v.Duration).Sum(v => v.Ticks));
    
    public void AddCourse(Course course) => Courses.Add(course);
    public void AddCourse(string name, TimeSpan? duration) => Courses.Add(new Course(name, duration));
    public void AddRange(IEnumerable<Course> courses) => Courses.AddRange(courses);
}