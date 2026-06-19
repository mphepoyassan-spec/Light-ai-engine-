using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Eviction_Works_In_FIFO_Order()
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
    public void Clear_Flushes_Everything()
    {
        // Arrange
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 60);
        cache.Set("key1", "val1");

        // Act
        cache.Clear();

        // Assert
        Assert.Null(cache.Get("key1"));

        // Ensure queue is also cleared by adding more items
        cache.Set("key2", "val2");
        Assert.Equal("val2", cache.Get("key2"));
    }
}
