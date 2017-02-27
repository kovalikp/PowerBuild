// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

internal class FileUtilities
{
    internal static readonly StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;

    internal static string FixFilePath(string path)
    {
        return string.IsNullOrEmpty(path) || Path.DirectorySeparatorChar == '\\' ? path : path.Replace('\\', '/');
    }

    internal static bool IsSolutionFilename(string filename)
    {
        return string.Equals(Path.GetExtension(filename), ".sln", PathComparison);
    }
}