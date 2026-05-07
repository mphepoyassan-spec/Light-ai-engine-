using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_FIFO()
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
    public void Cache_HandlesUpdatedKey_FIFO()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1, it should now be "newer" than key2 in terms of eviction priority if we were using LRU,
        // but this is FIFO. However, our implementation ensures we don't accidentally evict the NEW version of key1
        // when the OLD version's entry in the queue comes up.

        cache.Set("key1", "val1-new");

        cache.Set("key3", "val3");

        // key2 was the oldest unique key, so it should be gone.
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val1-new", cache.Get("key1"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // expired immediately
        cache.Set("key1", "val1");
        Assert.Null(cache.Get("key1"));
    }
}
