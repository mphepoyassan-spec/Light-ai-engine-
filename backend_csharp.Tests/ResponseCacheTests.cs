using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldestItem_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3"); // Should evict key1

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_HandlesUpdatedKey_Correctly()
    {
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1-updated"); // Update key1, it's now "newer" than key2

        cache.Set("key3", "val3"); // Should evict key2, not key1

        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Expire immediately

        cache.Set("key1", "val1");

        Assert.Null(cache.Get("key1"));
    }
}
