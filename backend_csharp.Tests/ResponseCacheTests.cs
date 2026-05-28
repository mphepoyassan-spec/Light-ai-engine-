using LightAI.Backend.Services;
using Xunit;

namespace backend_csharp.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_ShouldEvictOldestItem_WhenMaxSizeIsExceeded()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3");

        // Assert
        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenItemIsExpired()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Force expiration

        // Act
        cache.Set("key1", "value1");

        // Assert
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Set_ShouldUpdateExistingKey_AndMaintainCorrectEvictionOrder()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 2, ttlSeconds: 10);

        // Act
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key1", "value1-updated"); // key1 is now the newest
        cache.Set("key3", "value3"); // Should evict key2

        // Assert
        Assert.Equal("value1-updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }
}
