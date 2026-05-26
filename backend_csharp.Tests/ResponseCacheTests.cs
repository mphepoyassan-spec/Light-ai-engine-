using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_RespectsMaxSize_FIFO()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);

        cache.Set("k1", "v1");
        cache.Set("k2", "v2");
        cache.Set("k3", "v3");

        Assert.Null(cache.Get("k1"));
        Assert.Equal("v2", cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }

    [Fact]
    public void Get_RespectsTTL()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 1); // 1 second TTL

        cache.Set("k1", "v1");
        Assert.Equal("v1", cache.Get("k1"));

        Thread.Sleep(1100); // Wait for TTL to expire

        Assert.Null(cache.Get("k1"));
    }

    [Fact]
    public void Set_UpdatesExistingKey_AndAdjustsEviction()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);

        cache.Set("k1", "v1");
        cache.Set("k2", "v2");

        // Update k1, it should now be "newer" than k2
        cache.Set("k1", "v1-updated");

        // Add k3, k2 should be evicted because it's the oldest now
        cache.Set("k3", "v3");

        Assert.Equal("v1-updated", cache.Get("k1"));
        Assert.Null(cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }
}
