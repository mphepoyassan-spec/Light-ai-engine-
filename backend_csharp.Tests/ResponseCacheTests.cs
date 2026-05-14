using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Set_UpdatesExistingKey_AndMaintainsFIFO()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1 - this should update its timestamp in the cache,
        // but the OLD entry for key1 is still in the eviction queue.
        // However, our logic should handle it.
        cache.Set("key1", "val1-updated");

        // At this point, queue has: (key1, T1), (key2, T2), (key1, T3)
        // Cache has: {key1: (val1-updated, T3), key2: (val2, T2)}

        // Add key3, triggers eviction.
        // TryDequeue gets (key1, T1). Cache has (key1, T3). T1 != T3, so key1 is NOT removed.
        // TryDequeue gets (key2, T2). Cache has (key2, T2). T2 == T2, so key2 IS removed.
        cache.Set("key3", "val3");

        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public async Task Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 1);

        cache.Set("key1", "val1");
        Assert.Equal("val1", cache.Get("key1"));

        await Task.Delay(1100);

        Assert.Null(cache.Get("key1"));
    }
}
