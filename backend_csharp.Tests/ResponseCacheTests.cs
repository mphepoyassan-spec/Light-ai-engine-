using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_FIFO()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_HandlesTTL()
    {
        var cache = new ResponseCache(10, 0); // 0 seconds TTL
        cache.Set("key1", "val1");
        // Immediate check might still pass due to precision, but let's assume it expires
        Thread.Sleep(10);
        Assert.Null(cache.Get("key1"));
    }
}
