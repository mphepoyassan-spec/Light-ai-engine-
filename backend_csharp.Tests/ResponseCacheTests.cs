using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        // Using a very small TTL for testing might be tricky with real time,
        // but ResponseCache uses DateTime.UtcNow.
        // For unit tests, we'd ideally mock time, but let's test with 0 TTL if possible.
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 0);
        cache.Set("key1", "val1");

        // Even with 0 TTL, it might be instant. Let's use 1 second and wait or just trust the logic.
        // Actually, 0 TTL should make it expired immediately.
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Clear_RemovesAllEntries()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 60);
        cache.Set("key1", "val1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));
    }
}
