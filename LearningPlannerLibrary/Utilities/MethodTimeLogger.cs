using System.Reflection;

namespace LearningPlannerLibrary.Utilities;

public static class MethodTimeLogger
{
    public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message)
    {
        Console.WriteLine("{0}.{1} : {2}",
            methodBase.DeclaringType!.Name, methodBase.Name, timeSpan);
    }
}