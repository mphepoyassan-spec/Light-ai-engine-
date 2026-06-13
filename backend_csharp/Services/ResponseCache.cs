using System.Collections.Concurrent;

namespace LightAI.Backend.Services;

public class ResponseCache
{
    private readonly int _maxSize;
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<string, (string Value, DateTime Timestamp)> _cache = new();
    private readonly ConcurrentQueue<string> _keys = new();

    public ResponseCache(int maxSize = 100, int ttlSeconds = 300)
    {
        _maxSize = maxSize;
        _ttl = TimeSpan.FromSeconds(ttlSeconds);
    }

    public string? Get(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (DateTime.UtcNow - item.Timestamp < _ttl)
            {
                return item.Value;
            }
            _cache.TryRemove(key, out _);
        }
        return null;
    }

    public void Set(string key, string value)
    {
        bool isNew = !_cache.ContainsKey(key);
        _cache[key] = (value, DateTime.UtcNow);

        if (isNew)
        {
            _keys.Enqueue(key);
        }

        if (_cache.Count > _maxSize)
        {
            if (_keys.TryDequeue(out var oldestKey))
            {
                _cache.TryRemove(oldestKey, out _);
            }
        }
    }

    public void Clear()
    {
        _cache.Clear();
        while (_keys.TryDequeue(out _)) { }
    }
}
