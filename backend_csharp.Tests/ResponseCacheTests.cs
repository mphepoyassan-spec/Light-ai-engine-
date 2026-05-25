using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenLimitReached()
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
    public void Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(10, 0); // 0 seconds TTL
        cache.Set("1", "one");

        // Wait a tiny bit just in case
        Thread.Sleep(10);

        Assert.Null(cache.Get("1"));
    }
}
