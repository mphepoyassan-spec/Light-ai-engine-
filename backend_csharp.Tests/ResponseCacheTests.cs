using LightAI.Backend.Services;
using Xunit;

namespace backend_csharp.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_ShouldEvictOldestItem_WhenMaxSizeIsExceeded()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3"); // This should evict key1

        // Assert
        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void Set_ShouldHandleUpdatedKeysCorrectlyDuringEviction()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 60);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key1", "value1-updated"); // key1 is now the newest
        cache.Set("key3", "value3"); // This should evict key2, not key1

        // Assert
        Assert.Null(cache.Get("key2"));
        Assert.Equal("value1-updated", cache.Get("key1"));
        Assert.Equal("value3", cache.Get("key3"));
    }
}
