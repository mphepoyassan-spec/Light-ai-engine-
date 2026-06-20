using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("3", "three");

        Assert.Null(cache.Get("1"));
        Assert.Equal("two", cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Immediately expired
        cache.Set("key", "value");
        Assert.Null(cache.Get("key"));
    }

    [Fact]
    public void Clear_FlushesCache()
    {
        var cache = new ResponseCache();
        cache.Set("key", "value");
        cache.Clear();
        Assert.Null(cache.Get("key"));
    }
}
