using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KazooDotNet.Utils
{
    public class OrderedDictionary<T1,T2> : IDictionary<T1,T2>
    {
        private readonly LinkedList<T1> _keys = new LinkedList<T1>();
        private readonly Dictionary<T1, T2> _dict = new Dictionary<T1, T2>();

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            foreach (var key in _keys)
                yield return new KeyValuePair<T1, T2>(key, _dict[key]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            _keys.Clear();
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item) => _dict.Contains(item);

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            _keys.Remove(item.Key);
            return _dict.Remove(item.Key);
        }

        public int Count => _dict.Count;
        public bool IsReadOnly => false;

        public void Add(T1 key, T2 value)
        {
            _dict.Add(key, value);
            _keys.AddLast(key);
        }

        public void AddAfter(T1 existingKey, T1 newKey, T2 value)
        {
            var node = GetNode(existingKey);
            _dict.Add(newKey, value);
            _keys.AddAfter(node, newKey);
        }

        public void AddBefore(T1 existingKey, T1 newKey, T2 value)
        {
            var node = GetNode(existingKey);
            _dict.Add(newKey, value);
            _keys.AddBefore(node, newKey);
        }

        public void AddFirst(T1 newKey, T2 value)
        {
            _dict.Add(newKey, value);
            _keys.AddFirst(newKey);
        }

        public void AddLast(T1 newKey, T2 value)
        {
            _dict.Add(newKey, value);
            _keys.AddLast(newKey);
        }

        public bool ContainsKey(T1 key) => _dict.ContainsKey(key);

        public bool Remove(T1 key)
        {
            _keys.Remove(key);
            return _dict.Remove(key);
        }

        public bool TryGetValue(T1 key, out T2 value) => _dict.TryGetValue(key, out value);

        public T2 this[T1 key]
        {
            get => _dict[key];
            set
            {
                if (!_keys.Contains(key))
                    _keys.AddLast(key);
                _dict[key] = value;
            }
        }

        public ICollection<T1> Keys => _keys;
        public ICollection<T2> Values => _keys.Select(k => _dict[k]).ToList();

        private LinkedListNode<T1> GetNode(T1 key) => 
            _keys.Find(key) ?? throw new ArgumentException($"{key} does not exist");

    }
}