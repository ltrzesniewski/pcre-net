using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PCRE.Internal
{
    internal class PriorityCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Func<TKey, TValue> _valueFactory;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly LinkedList<CacheItem> _cache = new LinkedList<CacheItem>();
        private int _cacheSize;
        private object _head;

        public PriorityCache(int cacheSize, Func<TKey, TValue> valueFactory, IEqualityComparer<TKey> keyComparer = null)
        {
            _valueFactory = valueFactory;
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            CacheSize = cacheSize;
        }

        public int CacheSize
        {
            get => _cacheSize;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Invalid cache size");

                lock (_cache)
                {
                    while (_cache.Count > value)
                        _cache.RemoveLast();

                    if (value == 0)
                        _head = null;

                    _cacheSize = value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_cache)
                {
                    return _cache.Count;
                }
            }
        }

        public TValue GetOrAdd(TKey key)
        {
            var keyHash = _keyComparer.GetHashCode(key);

            var head = (CacheItem)Volatile.Read(ref _head);
            if (head != null && head.KeyHashCode == keyHash && _keyComparer.Equals(head.Key, key))
                return head.Value;

            if (_cacheSize == 0)
                return _valueFactory(key);

            lock (_cache)
            {
                for (var node = _cache.First; node != null; node = node.Next)
                {
                    if (node.Value.KeyHashCode == keyHash && _keyComparer.Equals(node.Value.Key, key))
                    {
                        Volatile.Write(ref _head, node.Value);
                        _cache.Remove(node);
                        _cache.AddFirst(node);
                        return node.Value.Value;
                    }
                }
            }

            var item = new CacheItem(key, keyHash, _valueFactory(key));
            Volatile.Write(ref _head, item);

            lock (_cache)
            {
                _cache.AddFirst(item);
                if (_cache.Count > _cacheSize)
                    _cache.RemoveLast();
                return item.Value;
            }
        }

        private class CacheItem
        {
            public readonly TKey Key;
            public readonly int KeyHashCode;
            public readonly TValue Value;

            public CacheItem(TKey key, int keyHashCode, TValue value)
            {
                Key = key;
                KeyHashCode = keyHashCode;
                Value = value;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_cache)
            {
                return _cache.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i.Value))
                    .ToList()
                    .GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
