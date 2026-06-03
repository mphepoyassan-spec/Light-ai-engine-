using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        var cache = new ResponseCache(10, 1); // 1 second TTL
        cache.Set("key1", "value1");

        Assert.Equal("value1", cache.Get("key1"));

        System.Threading.Thread.Sleep(1100);

        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Cache_UpdatesExistingKey_WithoutChangingEvictionOrder()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");

        // Update key1
        cache.Set("key1", "value1-updated");

        // Add key3, should still evict key1 if we follow simple FIFO by first-add
        // Actually, my implementation enqueues only if TryAdd is true.
        // So key1 remains oldest.
        cache.Set("key3", "value3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }
}
