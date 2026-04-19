using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(2, 60);
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
        // Set TTL to 0 for immediate expiration
        var cache = new ResponseCache(10, 0);
        cache.Set("key", "value");

        // Give it a tiny bit of time if needed, but 0 should be immediate
        Assert.Null(cache.Get("key"));
    }
}
