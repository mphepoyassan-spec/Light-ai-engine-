using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_RespectsMaxSize_FIFO()
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
    public async Task Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(100, 1); // 1 second TTL
        cache.Set("key", "value");
        Assert.Equal("value", cache.Get("key"));

        await Task.Delay(1100);
        Assert.Null(cache.Get("key"));
    }

    [Fact]
    public void Clear_EmptyCache()
    {
        var cache = new ResponseCache(100, 60);
        cache.Set("key", "value");
        cache.Clear();
        Assert.Null(cache.Get("key"));

        // Ensure we can still use it
        cache.Set("new", "val");
        Assert.Equal("val", cache.Get("new"));
    }
}
