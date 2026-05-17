using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
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
    public void Set_UpdatesExistingKey_AndStillEvictsOldest()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1 - this should update its timestamp in _cache but we still have the old entry in _evictionQueue
        cache.Set("key1", "val1-updated");

        // Add key3 - should trigger eviction
        cache.Set("key3", "val3");

        // key2 was the oldest one that wasn't updated
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
