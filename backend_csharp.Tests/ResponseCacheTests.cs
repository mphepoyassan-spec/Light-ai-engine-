using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_WhenMaxSizeExceeded()
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
    public void Cache_DoesNotEvict_WhenKeyIsUpdated()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1_updated"); // key1 is now "newer" in the queue
        cache.Set("key3", "val3"); // Should evict key2, not key1

        // Assert
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val1_updated", cache.Get("key1"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Cache_RespectsTTL()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Expired immediately

        // Act
        cache.Set("key1", "val1");

        // Assert
        Assert.Null(cache.Get("key1"));
    }
}
