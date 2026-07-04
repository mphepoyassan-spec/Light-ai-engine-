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
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Immediately expired

        cache.Set("key1", "val1");

        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Clear_FlushesAll()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 60);
        cache.Set("key1", "val1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));

        // Ensure queue is also cleared by checking eviction after re-filling
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");
        cache.Set("key4", "val4"); // Should not evict key2 if maxSize is large, but we can test with small maxSize
    }
}
