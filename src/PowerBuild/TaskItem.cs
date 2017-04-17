// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Shared;

    /// <summary>
    /// Represents marshallable by value task item.
    /// </summary>
    [Serializable]
    public class TaskItem : ITaskItem
    {
        internal const string AccessedTime = nameof(AccessedTime);
        internal const string CreatedTime = nameof(CreatedTime);
        internal const string DefiningProjectDirectory = nameof(DefiningProjectDirectory);
        internal const string DefiningProjectExtension = nameof(DefiningProjectExtension);
        internal const string DefiningProjectFullPath = nameof(DefiningProjectFullPath);
        internal const string DefiningProjectName = nameof(DefiningProjectName);
        internal const string Directory = nameof(Directory);
        internal const string Extension = nameof(Extension);
        internal const string Filename = nameof(Filename);
        internal const string FullPath = nameof(FullPath);
        internal const string Identity = nameof(Identity);
        internal const string ModifiedTime = nameof(ModifiedTime);
        internal const string RecursiveDir = nameof(RecursiveDir);
        internal const string RelativeDir = nameof(RelativeDir);
        internal const string RootDir = nameof(RootDir);

        internal static readonly string[] All =
        {
            FullPath,
            RootDir,
            Filename,
            Extension,
            RelativeDir,
            Directory,
            RecursiveDir,
            Identity,
            ModifiedTime,
            CreatedTime,
            AccessedTime,
            DefiningProjectFullPath,
            DefiningProjectDirectory,
            DefiningProjectName,
            DefiningProjectExtension
        };

        private static readonly HashSet<string> TableOfItemSpecModifiers = new HashSet<string>(All, StringComparer.OrdinalIgnoreCase);
        private string _itemSpec;

        private Dictionary<string, string> _metadata;

        [NonSerialized]
        private ITaskItem _taskItem;

        public TaskItem()
        {
            _itemSpec = string.Empty;
        }

        public TaskItem(string itemSpec)
        {
            _itemSpec = itemSpec;
        }

        public TaskItem(string itemSpec, IDictionary itemMetadata)
            : this(itemSpec)
        {
            ErrorUtilities.VerifyThrowArgumentNull(itemMetadata, "itemMetadata");

            if (itemMetadata.Count > 0)
            {
                var metadata = GetCustomMetadata();

                foreach (DictionaryEntry singleMetadata in itemMetadata)
                {
                    // don't import metadata whose names clash with the names of reserved metadata
                    string key = (string)singleMetadata.Key;
                    if (!IsDerivableItemSpecModifier(key))
                    {
                        metadata[key] = (string)singleMetadata.Value ?? string.Empty;
                    }
                }
            }
        }

        public TaskItem(ITaskItem taskItem)
            : this(taskItem.ItemSpec, taskItem.CloneCustomMetadata())
        {
            _taskItem = taskItem;
        }

        public string ItemSpec
        {
            get
            {
                return _itemSpec;
            }

            set
            {
                _itemSpec = value;
                _taskItem = null;
            }
        }

        public int MetadataCount => GetTaskItem().MetadataCount;

        public ICollection MetadataNames => GetTaskItem().MetadataNames;

        public IDictionary CloneCustomMetadata() => GetTaskItem().CloneCustomMetadata();

        public void CopyMetadataTo(ITaskItem destinationItem) => GetTaskItem().CopyMetadataTo(destinationItem);

        public string GetMetadata(string metadataName)
        {
            try
            {
                return GetTaskItem().GetMetadata(metadataName);
            }
            catch
            {
                return null;
            }
        }

        public void RemoveMetadata(string metadataName)
        {
            GetTaskItem().RemoveMetadata(metadataName);
            GetCustomMetadata().Remove(metadataName);
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            GetTaskItem().SetMetadata(metadataName, metadataValue);
            GetCustomMetadata()[metadataName] = metadataValue ?? string.Empty;
        }

        /// <summary>
        /// Indicates if the given name is reserved for a derivable item-spec modifier.
        /// Derivable means it can be computed given a file name.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <returns>true, if name of a derivable modifier</returns>
        internal static bool IsDerivableItemSpecModifier(string name)
        {
            if (!TableOfItemSpecModifiers.Contains(name))
            {
                return false;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals("RecursiveDir"))
            {
                return false;
            }

            return true;
        }

        private Dictionary<string, string> GetCustomMetadata()
        {
            return _metadata ?? (_metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        private ITaskItem GetTaskItem()
        {
            return _taskItem ?? (_taskItem = new Microsoft.Build.Utilities.TaskItem(_itemSpec, GetCustomMetadata()));
        }
    }
}