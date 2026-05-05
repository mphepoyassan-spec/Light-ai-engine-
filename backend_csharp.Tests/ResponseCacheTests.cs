using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Get_ReturnsNull_WhenExpired()
    {
        var cache = new ResponseCache(10, 0); // 0 seconds TTL
        cache.Set("test", "value");
        // Give it a tiny bit of time to definitely be expired if DateTime.UtcNow is the same
        Thread.Sleep(10);
        var result = cache.Get("test");
        Assert.Null(result);
    }

    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("k1", "v1");
        cache.Set("k2", "v2");
        cache.Set("k3", "v3");

        Assert.Null(cache.Get("k1"));
        Assert.Equal("v2", cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }

    [Fact]
    public void Set_UpdatesTimestamp_AndPreventsEarlyEviction()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("k1", "v1");
        cache.Set("k2", "v2");

        // Update k1, so it should now be "newer" than k2 in terms of eviction priority if we use FIFO on updates
        // Actually my implementation is FIFO on insertion/update.
        // Let's check:
        // Set("k1", "v1") -> Queue: [(k1, T1)]
        // Set("k2", "v2") -> Queue: [(k1, T1), (k2, T2)]
        // Set("k1", "v1_new") -> Queue: [(k1, T1), (k2, T2), (k1, T3)]

        cache.Set("k1", "v1_new");

        // Set("k3", "v3") -> Queue: [(k1, T1), (k2, T2), (k1, T3), (k3, T4)]
        // _cache.Count is 2. While count > 2, dequeue.
        // Dequeue (k1, T1). current.Timestamp is T3. T3 != T1. Don't remove.
        // Dequeue (k2, T2). current.Timestamp is T2. T2 == T2. Remove k2!

        cache.Set("k3", "v3");

        Assert.Equal("v1_new", cache.Get("k1"));
        Assert.Null(cache.Get("k2"));
        Assert.Equal("v3", cache.Get("k3"));
    }
}
