using System.Collections.Concurrent;

namespace LightAI.Backend.Services;

public class ResponseCache
{
    private readonly int _maxSize;
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<string, (string Value, DateTime Timestamp)> _cache = new();
    private readonly ConcurrentQueue<(string Key, DateTime Timestamp)> _evictionQueue = new();

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
        var timestamp = DateTime.UtcNow;
        _cache[key] = (value, timestamp);
        _evictionQueue.Enqueue((key, timestamp));

        if (_cache.Count > _maxSize)
        {
            if (_evictionQueue.TryDequeue(out var oldest))
            {
                // Only remove from cache if the timestamp matches (avoids removing a newer version of the same key)
                if (_cache.TryGetValue(oldest.Key, out var current) && current.Timestamp == oldest.Timestamp)
                {
                    _cache.TryRemove(oldest.Key, out _);
                }
            }
        }
    }

    public void Clear()
    {
        _cache.Clear();
        while (_evictionQueue.TryDequeue(out _)) { }
    }
}
