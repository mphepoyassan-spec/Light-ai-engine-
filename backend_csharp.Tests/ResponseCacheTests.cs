using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldestItem_WhenMaxSizeExceeded()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3"); // Should evict key1

        // Assert
        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void Set_UpdatesExistingItem_AndMaintainsCorrectEviction()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key1", "value1-updated"); // Updates key1, new timestamp
        cache.Set("key3", "value3"); // Should evict key2 (oldest), not key1

        // Assert
        Assert.Equal("value1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }
}
