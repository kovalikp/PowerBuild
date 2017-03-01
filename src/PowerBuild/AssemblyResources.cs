// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Reflection;
using System.Resources;
using Microsoft.Build.Shared;

internal static class AssemblyResources
{
    private static readonly ConcurrentDictionary<string, string> ResourceCache = new ConcurrentDictionary<string, string>();
    private static readonly ResourceManager MSBuildStrings = new ResourceManager("PowerBuild.MSBuild.Strings", typeof(AssemblyResources).GetTypeInfo().Assembly);
    private static readonly ResourceManager SharedStrings = new ResourceManager("PowerBuild.Shared.Strings.shared", typeof(AssemblyResources).GetTypeInfo().Assembly);

    public static string GetString(string resourceName)
    {
        return ResourceCache.GetOrAdd(resourceName, GetResource);
    }

    private static string GetResource(string name)
    {
        var resource = MSBuildStrings.GetString(name);
        if (resource != null)
        {
            return resource;
        }

        resource = SharedStrings.GetString(name);

        ErrorUtilities.VerifyThrow(resource != null, "Missing resource '{0}'", name);

        return resource;
    }
}