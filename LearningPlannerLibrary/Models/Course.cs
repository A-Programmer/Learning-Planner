namespace LearningPlannerLibrary.Models;

public sealed class Course : BaseModel
{
    public Course(string name, TimeSpan? duration = null)
        : base(name, duration)
    {
        
    }

    public List<Module> Modules { get; set; } = new();
    
    public TimeSpan GetDuration() => new TimeSpan(Modules.Select(v => v.Duration).Sum(v => v.Ticks));
    
    public void AddModule(Module module) => Modules.Add(module);
    public void AddModule(string name, TimeSpan? duration) => Modules.Add(new Module(name, duration));
    public void AddRange(IEnumerable<Module> modules) => Modules.AddRange(modules);

    public void Sort() => Modules = Modules.OrderBy(m => m.Name).ToList();
}