// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Generic Dictionary helper class that handles serialization of keys and values into lists before/after serialization time since Dictionary by itself is not Serializable.
    /// Extends C# Dictionary class to support typical API access methods
    /// </summary>
    /// <typeparam name="TKey">Key type for Dictionary</typeparam>
    /// <typeparam name="TValue">Value type for Dictionary</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SerializableDictionaryEntry> entries = new List<SerializableDictionaryEntry>();

        private readonly List<SerializableDictionaryEntry> nonNullEntries = new List<SerializableDictionaryEntry>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            nonNullEntries.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                nonNullEntries.Add(new SerializableDictionaryEntry(pair.Key, pair.Value));
            }

            int entriesCount = entries.Count;
            for (int i = 0; i < entriesCount; i++)
            {
                SerializableDictionaryEntry entry = entries[i];

                if (entry.Key != null && !nonNullEntries.Contains(entry))
                {
                    entries.Remove(entry);
                    i--;
                    entriesCount--;
                }
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            nonNullEntries.Clear();

            foreach (SerializableDictionaryEntry entry in entries)
            {
                if (entry.Key != null)
                {
                    nonNullEntries.Add(entry);
                }
            }

            foreach (SerializableDictionaryEntry entry in nonNullEntries)
            {
                Add(entry.Key, entry.Value);
            }
        }

        [Serializable]
        private struct SerializableDictionaryEntry
        {
            [SerializeField]
            private TKey key;

            public TKey Key => key;

            [SerializeField]
            private TValue value;

            public TValue Value => value;

            public SerializableDictionaryEntry(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}
