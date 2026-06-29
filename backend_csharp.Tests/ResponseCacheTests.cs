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
    public void Set_UpdatesExistingKey_DoesNotChangeOrder()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1-updated");
        cache.Set("key3", "val3");

        // If key1 update didn't move it to the end of the FIFO queue,
        // it should have been evicted when key3 was added because it was the oldest.
        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Clear_FlushesAll()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 60);
        cache.Set("key1", "val1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));
    }
}
