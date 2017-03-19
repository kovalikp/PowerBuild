// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class ResultsByTarget :
        IReadOnlyDictionary<string, TargetResult>
    {
        private readonly IDictionary<string, TargetResult> _dictionary;

        public ResultsByTarget()
        {
        }

        public ResultsByTarget(IDictionary<string, TargetResult> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count
            => _dictionary.Count;

        public IEnumerable<string> Keys
            => _dictionary.Keys;

        public IEnumerable<TargetResult> Values
            => _dictionary.Values;

        public TargetResult this[string key]
            => _dictionary[key];

        public bool ContainsKey(string key)
            => _dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, TargetResult>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        public bool TryGetValue(string key, out TargetResult value)
            => _dictionary.TryGetValue(key, out value);
    }
}