using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        var cache = new ResponseCache(100, -1); // Immediately expired
        cache.Set("key1", "val1");
        Assert.Null(cache.Get("key1"));
    }
}
