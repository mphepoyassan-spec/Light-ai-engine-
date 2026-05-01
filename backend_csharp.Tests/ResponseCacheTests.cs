using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_RespectsMaxSize_FIFO()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3"); // Should evict key1

        // Assert
        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 0); // 0 TTL

        // Act
        cache.Set("key1", "val1");
        // Tiny delay to ensure TTL expires if clock is fast,
        // but with 0 TTL it should be expired immediately.

        // Assert
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Cache_UpdateKey_MaintainsCorrectEviction()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1-updated"); // Updates key1, should still be in cache and might move its "old" entry to front of queue
        cache.Set("key3", "val3"); // Should evict key2 (the oldest non-updated entry)

        // Assert
        Assert.Equal("val1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
