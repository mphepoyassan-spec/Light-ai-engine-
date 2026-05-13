using LightAI.Backend.Services;
using Xunit;
using System;
using System.Threading;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Set_HandlesUpdatedKeys_Correctly()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Wait a bit to ensure timestamp difference if needed (though UtcNow is enough)
        Thread.Sleep(10);

        cache.Set("key1", "val1_updated"); // This should be enqueued again
        cache.Set("key3", "val3"); // This should evict the OLDEST key2, not key1 because key1 was updated

        Assert.Equal("val1_updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 1);
        cache.Set("key1", "val1");

        Thread.Sleep(1100); // Wait for expiration

        Assert.Null(cache.Get("key1"));
    }
}
