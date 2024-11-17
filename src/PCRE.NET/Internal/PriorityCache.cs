using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PCRE.Internal;

internal class PriorityCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly Func<TKey, TValue> _valueFactory;
    private readonly IEqualityComparer<TKey> _keyComparer;
    private readonly LinkedList<CacheItem> _cache = new();

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    private int _cacheSize;
    private object? _head;

    public PriorityCache(int cacheSize, Func<TKey, TValue> valueFactory, IEqualityComparer<TKey>? keyComparer = null)
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
                throw new ArgumentException("Invalid cache size.");

            lock (_lock)
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
            lock (_lock)
            {
                return _cache.Count;
            }
        }
    }

    public TValue GetOrAdd(TKey key)
    {
        var keyHash = _keyComparer.GetHashCode(key);

        var head = (CacheItem?)Volatile.Read(ref _head);
        if (head != null && head.KeyHashCode == keyHash && _keyComparer.Equals(head.Key, key))
            return head.Value;

        if (_cacheSize == 0)
            return _valueFactory(key);

        lock (_lock)
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

        lock (_lock)
        {
            _cache.AddFirst(item);
            if (_cache.Count > _cacheSize)
                _cache.RemoveLast();
            return item.Value;
        }
    }

    private class CacheItem(TKey key, int keyHashCode, TValue value)
    {
        public readonly TKey Key = key;
        public readonly int KeyHashCode = keyHashCode;
        public readonly TValue Value = value;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        lock (_lock)
        {
            return _cache.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i.Value))
                         .ToList()
                         .GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
