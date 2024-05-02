namespace LearningPlannerLibrary.Utilities;

public static class VideoHelper
{
    private static string[] mediaExtensions = {
        ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", //etc
        ".AVI", ".MP4", ".DIVX", ".WMV", ".MKV", //etc
    };

    public static bool IsMediaFile(string path) => mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

}