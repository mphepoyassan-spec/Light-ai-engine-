using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldestWhenFull()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 100);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ReturnsNullAfterExpiration()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 0); // Immediate expiration
        cache.Set("key1", "val1");

        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Set_UpdatesExistingKeyAndResetsEviction()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 100);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1, it should no longer be the oldest for eviction
        cache.Set("key1", "val1-updated");

        cache.Set("key3", "val3");

        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
