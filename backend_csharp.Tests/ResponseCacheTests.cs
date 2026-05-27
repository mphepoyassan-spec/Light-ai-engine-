using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenFull()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "v1");
        cache.Set("2", "v2");
        cache.Set("3", "v3");

        Assert.Null(cache.Get("1"));
        Assert.Equal("v2", cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }

    [Fact]
    public void Set_UpdatesExistingKey_AndAdjustsEvictionOrder()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "v1");
        cache.Set("2", "v2");

        // Update "1", should now be "newer" than "2" in some sense,
        // but our FIFO is strictly on Set call.
        // Wait, if we update "1", it enqueues a new entry.
        cache.Set("1", "v1-new");

        cache.Set("3", "v3");

        // After adding 3, either 1 or 2 should be evicted.
        // Queue was: (1, t1), (2, t2), (1, t3)
        // Count is 3, so it tries to dequeue.
        // Dequeues (1, t1). Since cache["1"].Timestamp is t3, it's NOT removed.
        // Count is still 3? No, the loop is while(count > maxSize).
        // It should dequeue again.
        // Dequeues (2, t2). cache["2"].Timestamp IS t2. Removed.
        // Count is 2. Loop ends.

        Assert.Equal("v1-new", cache.Get("1"));
        Assert.Null(cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }
}
