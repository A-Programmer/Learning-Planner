using System.Text.Json;
using static LearningPlannerLibrary.LengthCalculator.VideoLengthCalculator;
namespace VideoLengthCalculator;

class Program
{

    static async Task Main(string[] args)
    {
        // Get the path from args:
        string myPath = args[0];
        
        string videoPath = "Video File Path";
        string modulePath = "Module Path";
        string coursePath = "Course Path";

        string learningPathPath = "Path Path";
        
        string nonStructuredPath = "NonStructured Path";

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

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}