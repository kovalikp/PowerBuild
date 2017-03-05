// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using Microsoft.Build.Framework;

    public sealed class TaskItemAdapter : PSPropertyAdapter
    {
        public static string GetItemSpec(PSObject obj)
        {
            return (obj.BaseObject as ITaskItem)?.ItemSpec;
        }

        public static int GetMetadataCount(PSObject obj)
        {
            return (obj.BaseObject as ITaskItem)?.MetadataCount ?? 0;
        }

        public static ICollection GetMetadataNames(PSObject obj)
        {
            return (obj.BaseObject as ITaskItem)?.MetadataNames;
        }

        public static string GetName(PSObject obj)
        {
            var taskItem = obj.BaseObject as ITaskItem;
            if (taskItem == null)
            {
                return null;
            }

            var name = taskItem.GetMetadata("Filename") + taskItem.GetMetadata("Extension");
            if (string.IsNullOrEmpty(name))
            {
                name = taskItem.ItemSpec;
            }

            return name;
        }

        public override Collection<PSAdaptedProperty> GetProperties(object baseObject)
        {
            var result = new Collection<PSAdaptedProperty>();
            var taskItem = (ITaskItem)baseObject;
            foreach (string metadataName in taskItem.MetadataNames)
            {
                result.Add(new PSAdaptedProperty(metadataName, metadataName));
            }

            return result;
        }

        public override PSAdaptedProperty GetProperty(object baseObject, string propertyName)
        {
            return new PSAdaptedProperty(propertyName, propertyName);
        }

        public override string GetPropertyTypeName(PSAdaptedProperty adaptedProperty)
        {
            return typeof(string).ToString();
        }

        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            var taskItem = adaptedProperty.BaseObject as ITaskItem;
            return taskItem?.GetMetadata((string)adaptedProperty.Tag);
        }

        public override bool IsGettable(PSAdaptedProperty adaptedProperty)
        {
            return true;
        }

        public override bool IsSettable(PSAdaptedProperty adaptedProperty)
        {
            return false;
        }

        public override void SetPropertyValue(PSAdaptedProperty adaptedProperty, object value)
        {
            // pass
        }
    }
}