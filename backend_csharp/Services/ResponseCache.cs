using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using LightAI.Backend.Models;

namespace LightAI.Backend.Services;

public class ResponseCache
{
    private readonly int _maxSize;
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<string, (string Value, DateTime Timestamp)> _cache = new();

    public ResponseCache(IOptions<LightAIOptions> options, int maxSize = 100)
    {
        _maxSize = maxSize;
        _ttl = TimeSpan.FromSeconds(options.Value.CacheTTLSeconds);
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
        _cache[key] = (value, DateTime.UtcNow);

        if (_cache.Count > _maxSize)
        {
            var oldest = _cache.OrderBy(kvp => kvp.Value.Timestamp).FirstOrDefault();
            if (oldest.Key != null)
            {
                _cache.TryRemove(oldest.Key, out _);
            }
        }
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
