using System.Text.Json;
using static LearningPlannerLibrary.LengthCalculator.VideoLengthCalculator;
namespace VideoLengthCalculator;

class Program
{

    static async Task Main(string[] args)
    {
        
        string videoPath = "/Users/sadin/Learning/Pluralsight/My Skills/ASPNET-Microservices/Advanced/08 - Implementing Cross-cutting Concerns for ASP.NET Core 3 Microservices/01. Implementing Logging/01. Module Introduction.mp4";
        string modulePath = "/Users/sadin/Learning/Pluralsight/My Skills/ASPNET-Microservices/Advanced/08 - Implementing Cross-cutting Concerns for ASP.NET Core 3 Microservices/01. Implementing Logging/";
        string coursePath = "/Users/sadin/Learning/Pluralsight/My Skills/ASPNET-Microservices/Advanced/08 - Implementing Cross-cutting Concerns for ASP.NET Core 3 Microservices/";

        string learningPathPath = "/Users/sadin/Learning/Pluralsight/My Skills/ASPNET-Microservices/Advanced/";
        
        string nonStructuredPath = "/Users/sadin/Learning/Pluralsight/My Skills/";

        // var fullPath = Path.GetFullPath(path);
        // var pathValidation = Path.IsPathFullyQualified(path);
        // var fullPathValidation = Path.IsPathFullyQualified(fullPath);
        //
        // Console.WriteLine(path);
        // Console.WriteLine(fullPath);
        // Console.WriteLine(pathValidation);
        // Console.WriteLine(fullPathValidation);
        // Console.WriteLine(Path.PathSeparator);
        // Console.WriteLine(Path.DirectorySeparatorChar);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        
        
        var singleVid = await GetVideoLengthAsync(videoPath);

        FileStream singleVidFile = File.OpenWrite("SingleVideo.json");
        using (StreamWriter sw = new StreamWriter(singleVidFile))
        {
            await sw.WriteAsync(JsonSerializer.Serialize(singleVid, options));
        }
        
        
        
        var moduleVideos = await GetModuleLengthAsync(modulePath);
        
        FileStream moduleVideosFile = File.OpenWrite("ModuleVideo.json");
        using (StreamWriter sw = new StreamWriter(moduleVideosFile))
        {
            await sw.WriteAsync(JsonSerializer.Serialize(moduleVideos, options));
        }
        
        
        
        var courseVideos = await GetCourseLengthAsync(coursePath);

        FileStream courseVideoFiles = File.OpenWrite("CourseVideo.json");
        using (StreamWriter sw = new StreamWriter(courseVideoFiles))
        {
            await sw.WriteAsync(JsonSerializer.Serialize(courseVideos, options));
        }
        
        

        var learningPathVideos = await GetPathLengthAsync(learningPathPath);
        
        Console.WriteLine(learningPathVideos.GetDuration());
        foreach (var course in learningPathVideos.Courses)
        {
            Console.WriteLine($"\t{course.GetDuration()}");
        }
        
        FileStream learningPathFile = File.OpenWrite("LearningPathVideo.json");
        using (StreamWriter sw = new StreamWriter(learningPathFile))
        {
            await sw.WriteAsync(JsonSerializer.Serialize(learningPathVideos, options));
        }
        
        
        var nonStructured = await GetNonStructuredPathLength(nonStructuredPath);
        
        FileStream nonStructuredFile = File.OpenWrite("NonStructuredVideo.json");
        using (StreamWriter sw = new StreamWriter(nonStructuredFile))
        {
            await sw.WriteAsync(JsonSerializer.Serialize(nonStructured, options));
        }
        
        // Console.WriteLine(duration);
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}