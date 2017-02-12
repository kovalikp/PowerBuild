using System.Collections.Concurrent;
using System.Reflection;

internal static class AssemblyResources
{
    private static ConcurrentDictionary<string, string> _resourceCache = new ConcurrentDictionary<string, string>();

    public static string GetString(string resourceName)
    {
        return _resourceCache.GetOrAdd(resourceName, GetResource);
    }

    private static string GetResource(string resourceName)
    {
        var formatting = typeof(Microsoft.Build.Logging.ConsoleLogger).Assembly.GetType("Microsoft.Build.Shared.AssemblyResources");
        var getStringMethod = formatting.GetMethod("GetString", BindingFlags.Static | BindingFlags.NonPublic);
        return (string)getStringMethod.Invoke(null, new object[] { resourceName });
    }
}