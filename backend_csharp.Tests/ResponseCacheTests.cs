using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "v1");
        cache.Set("2", "v2");
        cache.Set("3", "v3");

        Assert.Null(cache.Get("1"));
        Assert.Equal("v2", cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }

    [Fact]
    public void Set_HandlesUpdatedKeys_Correctly()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "v1");
        cache.Set("2", "v2");
        cache.Set("1", "v1-new"); // Update "1", it's now the newest
        cache.Set("3", "v3");     // Should evict "2"

        Assert.Equal("v1-new", cache.Get("1"));
        Assert.Null(cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }
}
