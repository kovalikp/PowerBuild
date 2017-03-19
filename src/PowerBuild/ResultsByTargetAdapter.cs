// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;

    public class ResultsByTargetAdapter : PSPropertyAdapter
    {
        public static int GetCount(PSObject obj)
        {
            return ((ResultsByTarget)obj.BaseObject).Count;
        }

        public static IEnumerable<string> GetKeys(PSObject obj)
        {
            return ((ResultsByTarget)obj.BaseObject).Keys;
        }

        public static IEnumerable<TargetResult> GetValues(PSObject obj)
        {
            return ((ResultsByTarget)obj.BaseObject).Values;
        }

        public override Collection<PSAdaptedProperty> GetProperties(object baseObject)
        {
            var result = new Collection<PSAdaptedProperty>();
            var resultsByTarget = (ResultsByTarget)baseObject;
            foreach (string key in resultsByTarget.Keys)
            {
                result.Add(new PSAdaptedProperty(key, key));
            }

            return result;
        }

        public override PSAdaptedProperty GetProperty(object baseObject, string propertyName)
        {
            return new PSAdaptedProperty(propertyName, propertyName);
        }

        public override string GetPropertyTypeName(PSAdaptedProperty adaptedProperty)
        {
            var type = typeof(TargetResult);
            return typeof(TargetResult).FullName;
        }

        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            var resultsByTarget = (ResultsByTarget)adaptedProperty.BaseObject;
            return resultsByTarget[(string)adaptedProperty.Tag];
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