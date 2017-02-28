// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Reflection;

internal static class AssemblyResources
{
    private static readonly ConcurrentDictionary<string, string> _resourceCache = new ConcurrentDictionary<string, string>();

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