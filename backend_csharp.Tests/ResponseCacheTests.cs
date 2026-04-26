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
    public void Set_HandlesUpdatesCorrectly()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1-updated"); // Updates key1, timestamp changes
        cache.Set("key3", "val3");

        // After key3 is added, key2 should be evicted because it's the oldest now
        // key1 was updated so it's "newer" than key2
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
