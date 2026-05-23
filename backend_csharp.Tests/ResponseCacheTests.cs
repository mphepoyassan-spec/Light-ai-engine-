using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 100);
        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("3", "three");

        Assert.Null(cache.Get("1"));
        Assert.Equal("two", cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public async Task Get_ReturnsNull_WhenTTLPassed()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 1);
        cache.Set("key", "value");
        Assert.Equal("value", cache.Get("key"));

        await Task.Delay(1100);

        Assert.Null(cache.Get("key"));
    }

    [Fact]
    public void Set_UpdatesExistingKey_AndMaintainsFIFO()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 100);
        cache.Set("1", "one");
        cache.Set("2", "two");

        // Update "1", it should now be "newer" than "2" in terms of eviction if we were doing LRU,
        // but we are doing FIFO based on SET time.
        // Actually, my implementation enqueues on EVERY set.
        // So "1" is in queue twice.

        cache.Set("1", "one-updated");
        cache.Set("3", "three"); // This should trigger eviction.

        // Queue: (1, t1), (2, t2), (1, t3)
        // Count is 3. Max is 2.
        // Dequeue (1, t1). Current in cache for "1" has t3. Skip.
        // Dequeue (2, t2). Current in cache for "2" has t2. Evict "2".
        // Count is 2. Stop.

        Assert.Null(cache.Get("2"));
        Assert.Equal("one-updated", cache.Get("1"));
        Assert.Equal("three", cache.Get("3"));
    }
}
