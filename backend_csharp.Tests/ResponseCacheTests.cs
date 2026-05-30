using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenFull()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "A");
        cache.Set("2", "B");
        cache.Set("3", "C");

        Assert.Null(cache.Get("1"));
        Assert.Equal("B", cache.Get("2"));
        Assert.Equal("C", cache.Get("3"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        // Using 0 TTL to force expiration
        var cache = new ResponseCache(10, 0);
        cache.Set("key", "value");

        // Wait a tiny bit just in case
        Thread.Sleep(10);

        Assert.Null(cache.Get("key"));
    }
}
