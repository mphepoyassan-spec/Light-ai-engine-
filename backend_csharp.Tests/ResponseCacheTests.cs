using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_WhenMaxSizeReached()
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
    public void Cache_HandlesUpdatedKeys_Correctly()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1, making it "newest" in terms of timestamp in the cache
        // but it still has an old entry in the eviction queue
        cache.Set("key1", "val1-updated");

        // This should evict key2, because key1 was updated and its newest timestamp doesn't match the one in the queue for the first entry
        cache.Set("key3", "val3");

        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        var cache = new ResponseCache(10, 0); // 0 seconds TTL
        cache.Set("key1", "val1");

        // Should be expired immediately
        Assert.Null(cache.Get("key1"));
    }
}
