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
    public class TaskItem : ITaskItem, ITaskItem2
    {
        /// <summary>
        /// The custom metadata
        /// </summary>
        private Dictionary<string, string> _customEscapedMetadata = null;

        /// <summary>
        /// The full path to the project that originally defined this item.
        /// </summary>
        private string _escapedDefiningProject = null;

        /// <summary>
        /// The item spec
        /// </summary>
        private string _escapedItemSpec = null;

        private bool _escapedItemSpecIsValidPath;

        /// <summary>
        /// Cache for fullpath metadata
        /// </summary>
        private string _fullPath;

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public TaskItem(string escapedItemSpec, string escapedDefiningProject, Dictionary<string, string> escapedMetadata)
        {
            ErrorUtilities.VerifyThrowInternalNull(escapedItemSpec, "escapedItemSpec");

            _escapedItemSpec = escapedItemSpec;
            _escapedDefiningProject = escapedDefiningProject;
            _customEscapedMetadata = escapedMetadata;
            try
            {
                FileUtilities.ItemSpecModifiers.GetItemSpecModifier(
                    null,
                    _escapedItemSpec,
                    _escapedDefiningProject,
                    FileUtilities.ItemSpecModifiers.FullPath,
                    ref _fullPath);

                _escapedItemSpecIsValidPath = true;
            }
            catch
            {
                _escapedItemSpecIsValidPath = false;
            }
        }

        /// <summary>
        /// Returns the escaped version of this item's ItemSpec
        /// </summary>
        string ITaskItem2.EvaluatedIncludeEscaped
        {
            get
            {
                return _escapedItemSpec;
            }

            set
            {
                _escapedItemSpec = value;
            }
        }

        public string ItemSpec
        {
            get
            {
                return (_escapedItemSpec == null) ? string.Empty : EscapingUtilities.UnescapeAll(_escapedItemSpec);
            }

            set
            {
                _escapedItemSpec = value;
            }
        }

        /// <summary>
        /// Gets the number of pieces of metadata on the item. Includes
        /// both custom and built-in metadata.  Used only for unit testing.
        /// </summary>
        /// <value>Count of pieces of metadata.</value>
        public int MetadataCount
        {
            get
            {
                int count = (_customEscapedMetadata == null) ? 0 : _customEscapedMetadata.Count;
                if (_escapedItemSpecIsValidPath)
                {
                    count = count + FileUtilities.ItemSpecModifiers.All.Length;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the names of all the metadata on the item.
        /// Includes the built-in metadata like "FullPath".
        /// </summary>
        /// <value>The list of metadata names.</value>
        public ICollection MetadataNames
        {
            get
            {
                List<string> metadataNames = (_customEscapedMetadata == null) ? new List<string>() : new List<string>(_customEscapedMetadata.Keys);
                if (_escapedItemSpecIsValidPath)
                {
                    metadataNames.AddRange(FileUtilities.ItemSpecModifiers.All);
                }

                return metadataNames;
            }
        }

        /// <summary>
        /// Get the collection of custom metadata. This does not include built-in metadata.
        /// </summary>
        /// <remarks>
        /// RECOMMENDED GUIDELINES FOR METHOD IMPLEMENTATIONS:
        /// 1) this method should return a clone of the metadata
        /// 2) writing to this dictionary should not be reflected in the underlying item.
        /// </remarks>
        /// <returns>Dictionary of cloned metadata</returns>
        public IDictionary CloneCustomMetadata()
        {
            IDictionary<string, string> clonedMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (_customEscapedMetadata != null)
            {
                foreach (KeyValuePair<string, string> metadatum in _customEscapedMetadata)
                {
                    clonedMetadata.Add(metadatum.Key, EscapingUtilities.UnescapeAll(metadatum.Value));
                }
            }

            return (IDictionary)clonedMetadata;
        }

        /// <summary>
        /// Returns a dictionary containing all metadata and their escaped forms.
        /// </summary>
        IDictionary ITaskItem2.CloneCustomMetadataEscaped()
        {
            IDictionary clonedDictionary = new Dictionary<string, string>(_customEscapedMetadata);
            return clonedDictionary;
        }

        /// <summary>
        /// Allows custom metadata on the item to be copied to another item.
        /// </summary>
        /// <remarks>
        /// RECOMMENDED GUIDELINES FOR METHOD IMPLEMENTATIONS:
        /// 1) this method should NOT copy over the item-spec
        /// 2) if a particular piece of metadata already exists on the destination item, it should NOT be overwritten
        /// 3) if there are pieces of metadata on the item that make no semantic sense on the destination item, they should NOT be copied
        /// </remarks>
        /// <param name="destinationItem">The item to copy metadata to.</param>
        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            ErrorUtilities.VerifyThrowArgumentNull(destinationItem, "destinationItem");

            // also copy the original item-spec under a "magic" metadata -- this is useful for tasks that forward metadata
            // between items, and need to know the source item where the metadata came from
            string originalItemSpec = destinationItem.GetMetadata("OriginalItemSpec");

            if (_customEscapedMetadata != null)
            {
                foreach (KeyValuePair<string, string> entry in _customEscapedMetadata)
                {
                    string value = destinationItem.GetMetadata(entry.Key);

                    if (string.IsNullOrEmpty(value))
                    {
                        destinationItem.SetMetadata(entry.Key, entry.Value);
                    }
                }
            }

            if (string.IsNullOrEmpty(originalItemSpec))
            {
                destinationItem.SetMetadata("OriginalItemSpec", EscapingUtilities.Escape(ItemSpec));
            }
        }

        /// <summary>
        /// Allows the values of metadata on the item to be queried.
        /// </summary>
        /// <param name="metadataName">The name of the metadata to retrieve.</param>
        /// <returns>The value of the specified metadata.</returns>
        public string GetMetadata(string metadataName)
        {
            string metadataValue = (this as ITaskItem2).GetMetadataValueEscaped(metadataName);
            return EscapingUtilities.UnescapeAll(metadataValue);
        }

        /// <summary>
        /// Returns the escaped value of the requested metadata name.
        /// </summary>
        string ITaskItem2.GetMetadataValueEscaped(string metadataName)
        {
            ErrorUtilities.VerifyThrowArgumentNull(metadataName, "metadataName");

            string metadataValue = null;

            if (FileUtilities.ItemSpecModifiers.IsDerivableItemSpecModifier(metadataName))
            {
                // FileUtilities.GetItemSpecModifier is expecting escaped data, which we assume we already are.
                // Passing in a null for currentDirectory indicates we are already in the correct current directory
                metadataValue = FileUtilities.ItemSpecModifiers.GetItemSpecModifier(null, _escapedItemSpec, _escapedDefiningProject, metadataName, ref _fullPath);
            }
            else if (_customEscapedMetadata != null)
            {
                _customEscapedMetadata.TryGetValue(metadataName, out metadataValue);
            }

            return (metadataValue == null) ? string.Empty : metadataValue;
        }

        /// <summary>
        /// Allows the removal of custom metadata set on the item.
        /// </summary>
        /// <param name="metadataName">The name of the metadata to remove.</param>
        public void RemoveMetadata(string metadataName)
        {
            ErrorUtilities.VerifyThrowArgumentNull(metadataName, "metadataName");
            ErrorUtilities.VerifyThrowArgument(!FileUtilities.ItemSpecModifiers.IsItemSpecModifier(metadataName), "Shared.CannotChangeItemSpecModifiers", metadataName);

            if (_customEscapedMetadata == null)
            {
                return;
            }

            _customEscapedMetadata.Remove(metadataName);
        }

        /// <summary>
        /// Allows a piece of custom metadata to be set on the item.
        /// </summary>
        /// <param name="metadataName">The name of the metadata to set.</param>
        /// <param name="metadataValue">The metadata value.</param>
        public void SetMetadata(string metadataName, string metadataValue)
        {
            ErrorUtilities.VerifyThrowArgumentLength(metadataName, "metadataName");

            // Non-derivable metadata can only be set at construction time.
            // That's why this is IsItemSpecModifier and not IsDerivableItemSpecModifier.
            ErrorUtilities.VerifyThrowArgument(!FileUtilities.ItemSpecModifiers.IsDerivableItemSpecModifier(metadataName), "Shared.CannotChangeItemSpecModifiers", metadataName);

            _customEscapedMetadata = _customEscapedMetadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _customEscapedMetadata[metadataName] = metadataValue ?? string.Empty;
        }

        /// <summary>
        /// Sets the exact metadata value given to the metadata name requested.
        /// </summary>
        void ITaskItem2.SetMetadataValueLiteral(string metadataName, string metadataValue)
        {
            SetMetadata(metadataName, EscapingUtilities.Escape(metadataValue));
        }
    }
}