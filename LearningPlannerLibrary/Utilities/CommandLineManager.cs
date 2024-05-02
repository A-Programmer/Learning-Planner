using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LearningPlannerLibrary.Utilities;

public class CommandLineManager
{
    internal static async Task<string> ExecuteCommandAsync(string command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Process process = new();
            process.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "/bin/bash";
            process.StartInfo.Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c \"{command}\"" : $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            return output.Trim();
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }
}