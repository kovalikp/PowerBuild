// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using Microsoft.Build.Framework;

    [Serializable]
    public struct TaskItem : ITaskItem
    {
        private readonly ITaskItem _inneTaskItem;

        public TaskItem(ITaskItem inneTaskItem)
        {
            _inneTaskItem = inneTaskItem;
        }

        public string ItemSpec
        {
            get { return _inneTaskItem.ItemSpec; }
            set { _inneTaskItem.ItemSpec = value; }
        }

        public int MetadataCount => _inneTaskItem.MetadataCount;

        public ICollection MetadataNames => _inneTaskItem.MetadataNames;

        public IDictionary CloneCustomMetadata()
        {
            return _inneTaskItem.CloneCustomMetadata();
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            _inneTaskItem.CopyMetadataTo(destinationItem);
        }

        public string GetMetadata(string metadataName)
        {
            return _inneTaskItem.GetMetadata(metadataName);
        }

        public void RemoveMetadata(string metadataName)
        {
            _inneTaskItem.RemoveMetadata(metadataName);
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            _inneTaskItem.SetMetadata(metadataName, metadataValue);
        }
    }
}