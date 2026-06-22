using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void FIFO_Eviction_Works()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("k1", "v1");
        cache.Set("k2", "v2");
        cache.Set("k3", "v3");

        Assert.Null(cache.Get("k1"));
        Assert.Equal("v2", cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }

    [Fact]
    public void Clear_Works()
    {
        var cache = new ResponseCache(10, 300);
        cache.Set("k1", "v1");
        cache.Clear();
        Assert.Null(cache.Get("k1"));

        // Ensure queue is also cleared by checking eviction after clear
        cache = new ResponseCache(1, 300);
        cache.Set("old", "v");
        cache.Clear();
        cache.Set("new", "v2");
        Assert.Equal("v2", cache.Get("new"));
    }
}
