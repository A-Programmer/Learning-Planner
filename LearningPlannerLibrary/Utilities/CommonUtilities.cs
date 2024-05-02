namespace LearningPlannerLibrary.Utilities;

public static class CommonUtilities
{
    public static bool IsValidPath(this string filePath)
    {
        return Path.Exists(filePath);
    }
}