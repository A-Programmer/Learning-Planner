using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LearningPlannerLibrary.Models;
using LearningPlannerLibrary.Utilities;

namespace LearningPlannerLibrary.LengthCalculator;

public class VideoLengthCalculator
{
    /// <summary>
    /// Get a single video duration
    /// </summary>
    /// <param name="videoPath">Video Path based on the OS file system</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task of TimeSpan as video duration</returns>
    public static async Task<Video> GetVideoLengthAsync(string videoPath,
        CancellationToken cancellationToken = default)
    {
        if (!videoPath.IsValidPath())
            throw new FileNotFoundException(nameof(videoPath));

        if (!VideoHelper.IsMediaFile(videoPath))
            throw new NotSupportedException(Path.GetExtension(videoPath));
        
        string ffmpegPath = await FfmpegHelper.GetFfmpegPathAsync();

        if (!string.IsNullOrEmpty(ffmpegPath))
        {
            string validVideoPathInCommand = $@"""{videoPath}""";
            string command = $"\"{ffmpegPath}\" -i \"{validVideoPathInCommand}\" 2>&1 | grep 'Duration' | cut -d ' ' -f 4 | sed s/,//";

            string durationString = await CommandLineManager.ExecuteCommandAsync(command, cancellationToken);

            if (!TimeSpan.TryParse(durationString,
                   new CultureInfo("en-US"),
                   out var duration))
                throw new Exception("Duration could not be calculated.");

            return new Video(Path.GetFileName(videoPath), duration);
        }
        else
        {
            Console.WriteLine("Ffmpeg is not installed. Installing...");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await FfmpegHelper.InstallFfmpegWindowsAsync(cancellationToken);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                await FfmpegHelper.InstallFfmpegMacOsAsync(cancellationToken);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                await FfmpegHelper.InstallFfmpegLinuxAsync(cancellationToken);
            }
            else
            {
                Console.WriteLine("Ffmpeg is not supported on this operating system.");
            }
        }

        return await GetVideoLengthAsync(videoPath);
    }

    /// <summary>
    /// Returns a directory duration which contains videos.
    /// The directory constructor should be like the following:
    /// <br/>
    /// <code>
    ///     Directory <br/>
    ///     |____Video-1.mp4 <br/>
    ///     |____Video-2.mp4 <br/>
    ///     |____Video-3.mp4 <br/>
    ///  <br/>
    /// </code>
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Module> GetModuleLengthAsync(string directoryPath,
        CancellationToken cancellationToken = default)
    {
        Module module = new(new DirectoryInfo(Path.GetDirectoryName(directoryPath)).Name);

        await Parallel.ForEachAsync(GetModuleVideos(directoryPath, cancellationToken),  async (video, ct) =>
        {
            module.AddVideo(video);
        });
        
        return module;
    }

    /// <summary>
    /// Returns a course duration by searching it's nested directories.
    /// A course contains multiple modules and each module contains multiple videos
    /// The directory constructor should be like the following:
    /// <br/>
    /// <code>
    ///     CourseDirectoryName <br/>
    ///     |___ModuleDirectory-1 <br/>
    ///     |   |____Video-1.mp4 <br/>
    ///     |   |____Video-2.mp4 <br/>
    ///     |   |____Video-3.mp4 <br/>
    ///     | <br/>
    ///     |___ModuleDirectory-2 <br/>
    ///         |____Video-1.mp4 <br/>
    ///         |____Video-2.mp4 <br/>
    ///         |____Video-3.mp4 <br/>
    ///  <br/>
    /// </code>
    /// </summary>
    /// <param name="courseRootPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Course> GetCourseLengthAsync(string courseRootPath,
        CancellationToken cancellationToken = default)
    {
        if (!courseRootPath.IsValidPath())
            throw new DirectoryNotFoundException(nameof(courseRootPath));
        DirectoryInfo directoryInfo = new(courseRootPath);

        List<DirectoryInfo> courseDirectories = directoryInfo.GetDirectories().ToList();
        
        Course course = new(new DirectoryInfo(Path.GetDirectoryName(courseRootPath)).Name);

        await Parallel.ForEachAsync(courseDirectories, cancellationToken, async (di, ct) =>
        {
            course.AddModule(await GetModuleLengthAsync(di.FullName, ct));
        });
        
        return course;
    }

    /// <summary>
    /// Returns a path duration by searching it's nested directories.
    /// A path contains multiple courses and each course contains multiple modules and each module contains multiple videos
    /// The directory constructor should be like the following:
    /// <br/>
    /// <code>
    ///     PathDirectoryName
    ///     |
    ///     |____CourseDirectoryName-1 <br/>
    ///     |    |___ModuleDirectory-1 <br/>
    ///     |    |   |____Video-1.mp4 <br/>
    ///     |    |   |____Video-2.mp4 <br/>
    ///     |    |   |____Video-3.mp4 <br/>
    ///     |    | <br/>
    ///     |    |___ModuleDirectory-2 <br/>
    ///     |        |____Video-1.mp4 <br/>
    ///     |        |____Video-2.mp4 <br/>
    ///     |        |____Video-3.mp4 <br/>
    ///     |<br/>
    ///     |____CourseDirectoryName-2 <br/>
    ///     |    |___ModuleDirectory-1 <br/>
    ///     |    |   |____Video-1.mp4 <br/>
    ///     |    |   |____Video-2.mp4 <br/>
    ///     |    |   |____Video-3.mp4 <br/>
    ///     |    | <br/>
    ///     |    |___ModuleDirectory-2 <br/>
    ///     |        |____Video-1.mp4 <br/>
    ///     |        |____Video-2.mp4 <br/>
    ///     |        |____Video-3.mp4 <br/>
    ///  <br/>
    /// </code>
    /// </summary>
    /// <param name="pathRootPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<LearningPath> GetPathLengthAsync(string pathRootPath,
        CancellationToken cancellationToken = default)
    {
        if (!pathRootPath.IsValidPath())
            throw new DirectoryNotFoundException(nameof(pathRootPath));
        DirectoryInfo directoryInfo = new(pathRootPath);

        List<DirectoryInfo> pathDirectories = directoryInfo.GetDirectories().ToList();
        
        LearningPath path = new(new DirectoryInfo(Path.GetDirectoryName(pathRootPath)).Name);

        await Parallel.ForEachAsync(pathDirectories, cancellationToken, async (di, ct) =>
        {
            path.AddCourse(await GetCourseLengthAsync(di.FullName, ct));
        });
        return path;
    }

    /// <summary>
    /// Returns a Non-Structured duration by searching it's nested directories.
    /// A Non-Structured can contain as many as needed nested directories and videos (unknown depth)
    /// </summary>
    /// <param name="nonStructuredRootPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<NonStructuredDirectory> GetNonStructuredPathLength(string nonStructuredRootPath,
        CancellationToken cancellationToken = default)
    {
        DirectoryInfo directoryInfo = new(nonStructuredRootPath);
        
        List<DirectoryInfo> pathDirectories = directoryInfo.GetDirectories().ToList();
        
        NonStructuredDirectory nonStructuredDirectory = new(new DirectoryInfo(Path.GetDirectoryName(nonStructuredRootPath)).Name, new TimeSpan(0));
        
        await Parallel.ForEachAsync(GetModuleVideos(nonStructuredRootPath, cancellationToken), cancellationToken, async (video, ct) =>
        {
            nonStructuredDirectory.AddVideo(video);
        });
        
        if (pathDirectories.Count != 0)
        {
            await Parallel.ForEachAsync(pathDirectories, cancellationToken, async (di, ct) =>
            {
                nonStructuredDirectory.AddSubDirectory(await GetNonStructuredPathLength(di.FullName, ct)); 
            });
        }
        
        return nonStructuredDirectory;
    }
    

    private static async IAsyncEnumerable<Video> GetModuleVideos(string modulePath,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!modulePath.IsValidPath())
            throw new DirectoryNotFoundException(nameof(modulePath));
        DirectoryInfo directoryInfo = new(modulePath);

        var supportedVideos = directoryInfo
            .GetFiles()
            .Where(f => VideoHelper.IsMediaFile(f.Name))
            .ToList();

        List<Task<Video>> videos = new List<Task<Video>>();
        foreach (var file in supportedVideos)
        {
            videos.Add(GetVideoLengthAsync(file.FullName, cancellationToken));
        }

        while (videos.Any())
        {
            var completedVideoLengthCalculator = await Task.WhenAny(videos).ConfigureAwait(false);
            videos.Remove(completedVideoLengthCalculator);

            var video = await completedVideoLengthCalculator.ConfigureAwait(false);
            yield return video;
        }
    }
}