using System.Runtime.InteropServices;
using LearningPlannerLibrary.Utilities;

namespace LearningPlannerLibrary.LengthCalculator;

public static class FfmpegHelper
{
    internal static async Task<string> GetFfmpegPathAsync()
    {
        CancellationTokenSource ctSource = new();
        
        // Log the process
        string ffmpegCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
        return await CommandLineManager.ExecuteCommandAsync($"which {ffmpegCommand}", ctSource.Token);
    }

    internal static async Task InstallFfmpegWindowsAsync(CancellationToken cancellationToken = default)
    {
        // Log the process
        await CommandLineManager.ExecuteCommandAsync("choco install ffmpeg", cancellationToken);
    }

    internal static async Task InstallFfmpegMacOsAsync(CancellationToken cancellationToken = default)
    {
        // Log the process
        await CommandLineManager
            .ExecuteCommandAsync("sudo curl -s https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh | bash",
                cancellationToken);
        
        await CommandLineManager
            .ExecuteCommandAsync("brew install ffmpeg",
                cancellationToken);
    }

    internal static async Task InstallFfmpegLinuxAsync(CancellationToken cancellationToken = default)
    {
        // Log the process
        await CommandLineManager
            .ExecuteCommandAsync("sudo apt-get install ffmpeg",
                cancellationToken);
    }
}