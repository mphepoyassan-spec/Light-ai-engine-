using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Backend.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_ShouldReturnCachedValue()
    {
        var cache = new ResponseCache();
        cache.Set("test", "result");
        Assert.Equal("result", cache.Get("test"));
    }

    [Fact]
    public void Cache_ShouldRespectMaxSize_FIFO()
    {
        var cache = new ResponseCache(2);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_ShouldRespectTTL()
    {
        var cache = new ResponseCache(100, 1); // 1 second TTL
        cache.Set("test", "result");
        Assert.Equal("result", cache.Get("test"));

        Thread.Sleep(1100);
        Assert.Null(cache.Get("test"));
    }

    [Fact]
    public void Cache_UpdateShouldNotChangeFIFOOrder()
    {
        var cache = new ResponseCache(2);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1
        cache.Set("key1", "val1-updated");

        // Add key3, key1 should NOT be evicted because it was already there, but wait...
        // In my implementation, if it is an update, I don't re-enqueue.
        // So key1 is still at the head of the queue.
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
