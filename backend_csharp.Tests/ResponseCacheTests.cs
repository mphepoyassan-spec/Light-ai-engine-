using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "val1");
        cache.Set("2", "val2");
        cache.Set("3", "val3");

        Assert.Null(cache.Get("1"));
        Assert.Equal("val2", cache.Get("2"));
        Assert.Equal("val3", cache.Get("3"));
    }

    [Fact]
    public void Get_ReturnsNull_AfterTTL()
    {
        // Use a very short TTL for testing
        var cache = new ResponseCache(10, 0);
        cache.Set("key", "value");

        // Wait a bit to ensure TTL expires (though 0 should be immediate)
        System.Threading.Thread.Sleep(10);

        Assert.Null(cache.Get("key"));
    }
}
