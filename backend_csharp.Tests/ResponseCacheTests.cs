using LightAI.Backend.Services;
using Xunit;
using System.Threading;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "v1");
        cache.Set("2", "v2");
        cache.Set("3", "v3");

        Assert.Null(cache.Get("1"));
        Assert.Equal("v2", cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }

    [Fact]
    public void Set_UpdateKey_HandlesEvictionCorrectly()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "v1");
        cache.Set("2", "v2");

        // Update "1", it should now be considered "newer" in the eviction logic
        // Though our current FIFO is based on enqueue time, let's see.
        // Actually, my implementation enqueues on EVERY set.
        cache.Set("1", "v1-updated");

        cache.Set("3", "v3");

        // After setting "3", if "2" was the oldest it should be gone.
        // Queue has: (1, t1), (2, t2), (1, t3)
        // Set("3") happens. Count is 3.
        // Dequeue (1, t1). Cache has (1, t3). t1 != t3. Skip.
        // Dequeue (2, t2). Cache has (2, t2). t2 == t2. Remove "2".
        // Cache now has (1, t3) and (3, t4). Count is 2. Loop ends.

        Assert.Equal("v1-updated", cache.Get("1"));
        Assert.Null(cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }
}
