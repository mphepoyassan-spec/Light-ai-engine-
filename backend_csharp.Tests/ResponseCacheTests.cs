using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
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
    public void Get_ReturnsNull_WhenExpired()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: -1); // Immediately expired

        // Act
        cache.Set("key1", "val1");

        // Assert
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Clear_RemovesAllEntries()
    {
        // Arrange
        var cache = new ResponseCache();
        cache.Set("key1", "val1");

        // Act
        cache.Clear();

        // Assert
        Assert.Null(cache.Get("key1"));
    }
}
