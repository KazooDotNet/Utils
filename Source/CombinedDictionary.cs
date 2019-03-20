using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KazooDotNet.Utils
{
    public class CombinedDictionary<T,K> : IReadOnlyDictionary<T,K>
    {
        private IDictionary<T,K>[] _dictionaries;
        
        public CombinedDictionary(params IDictionary<T,K>[] dictionaries)
        {
            _dictionaries = dictionaries;
            Values = new K[]{};
        }
        
        public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int _count = -1;

        public int Count
        {
            get
            {
                if (_count > -1) return _count;
                var newCount = _dictionaries.Sum(d => d.Count);
                return _count = newCount;
            }
        }
        
        public bool ContainsKey(T key)
        {
            return _dictionaries.Any(d => d.ContainsKey(key));
        }

        public bool TryGetValue(T key, out K value)
        {
            foreach (var d in _dictionaries)
                if (d.ContainsKey(key))
                {
                    value = d[key];
                    return true;
                }
            
            value = default;
            return false;
        }

        public K this[T key]
        {
            get
            {
                foreach (var d in _dictionaries)
                    if (d.ContainsKey(key))
                        return d[key];
                throw new ArgumentException($"No included dictionaries contain key {key}");
            }
        }

        private T[] _keys;

        public IEnumerable<T> Keys
        {
            get
            {
                if (_keys != null) return _keys;
                var set = new HashSet<T>();
                foreach (var d in _dictionaries)
                foreach (var k in d.Keys)
                    set.Add(k);
                _keys = set.ToArray();
                return _keys;
            }
        }
        
        
        // TODO: figure out efficient way to emit values
        public IEnumerable<K> Values { get; }
    }
}