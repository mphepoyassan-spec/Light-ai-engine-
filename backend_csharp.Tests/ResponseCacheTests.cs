using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_WhenFull()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("k1", "v1");
        cache.Set("k2", "v2");
        cache.Set("k3", "v3");

        Assert.Null(cache.Get("k1"));
        Assert.Equal("v2", cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }

    [Fact]
    public void Cache_HandlesUpdatedKeys_Correctly()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("k1", "v1");
        cache.Set("k2", "v2");
        cache.Set("k1", "v1-updated");
        cache.Set("k3", "v3");

        // k2 should be evicted because it's the oldest now (k1 was updated)
        Assert.Equal("v1-updated", cache.Get("k1"));
        Assert.Null(cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }
}
