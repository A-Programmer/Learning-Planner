using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using BoardsLibrary;
using static LearningPlannerLibrary.LengthCalculator.VideoLengthCalculator;
namespace VideoLengthCalculator;

class Program
{
    static async Task Main(string[] args)
    {
        AzureBoardsService azBoard = new("PATH", "OrganizationURL", "Project Name");

        var workItems = await azBoard.ListAllWorkItems();
        
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        Console.WriteLine("Welcome to Video Length Calculator.");
        
        Options:
        
        Console.WriteLine("Please choose one option:");
        Console.WriteLine("\t1. Single Video Length");
        Console.WriteLine("\t2.Module Length");
        Console.WriteLine("\t3.Course Length");
        Console.WriteLine("\t4.Learning Path Length");

        string optionString = Console.ReadLine();
        if (!int.TryParse(optionString, out int option))
        {
            Console.WriteLine("Please enter the number of the option!");
            goto Options;
        }
        else
        {
            GetPath:

            Console.Write("Please enter the path:");
            string path = Console.ReadLine();
            if (!System.IO.Path.Exists(path))
            {
                Console.WriteLine("The path you entered is not exists.");
                goto GetPath;
            }

            switch (option)
            {
                case 1:
                {
                    var singleVideo = await GetVideoLengthAsync(path);
                    FileStream singleVidFile = File.OpenWrite("SingleVideo.json");
                    using (StreamWriter sw = new StreamWriter(singleVidFile))
                    {
                        await sw.WriteAsync(JsonSerializer.Serialize(path, options));
                    }
                    Console.WriteLine($"Total Duration: {singleVideo.Duration}");
                    break;
                }
                case 2:
                {
                    var moduleVideos = await GetModuleLengthAsync(path);
                    FileStream moduleVideosFile = File.OpenWrite("ModuleVideo.json");
                    using (StreamWriter sw = new StreamWriter(moduleVideosFile))
                    {
                        await sw.WriteAsync(JsonSerializer.Serialize(moduleVideos, options));
                    }
                    Console.WriteLine($"Total Duration: {moduleVideos.GetDuration()}");
                    break;
                }
                case 3:
                {
                    var courseVideos = await GetCourseLengthAsync(path);
                    FileStream courseVideoFiles = File.OpenWrite("CourseVideo.json");
                    using (StreamWriter sw = new StreamWriter(courseVideoFiles))
                    {
                        await sw.WriteAsync(JsonSerializer.Serialize(courseVideos, options));
                    }
                    Console.WriteLine($"Total Duration: {courseVideos.GetDuration()}");
                    break;
                }
                case 4:
                {
                    var learningPathVideos = await GetPathLengthAsync(path);
                    FileStream learningPathFile = File.OpenWrite("LearningPathVideo.json");
                    using (StreamWriter sw = new StreamWriter(learningPathFile))
                    {
                        await sw.WriteAsync(JsonSerializer.Serialize(learningPathVideos, options));
                    }
                    Console.WriteLine($"Total Duration: {learningPathVideos.GetDuration()}");
                    break;
                }
                case 5:
                {
                    var nonStructured = await GetNonStructuredPathLength(path);
                    FileStream nonStructuredFile = File.OpenWrite("NonStructuredVideo.json");
                    using (StreamWriter sw = new StreamWriter(nonStructuredFile))
                    {
                        await sw.WriteAsync(JsonSerializer.Serialize(nonStructured, options));
                    }
                    Console.WriteLine($"Total Duration: {nonStructured.GetDuration()}");
                    break;
                }
            }
        }
        
        Console.ReadKey();
    }
}