using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_RespectsMaxSize_FIFO()
    {
        var cache = new ResponseCache(maxSize: 2);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Set_HandlesUpdatedKeys_CorrectFIFO()
    {
        var cache = new ResponseCache(maxSize: 2);

        cache.Set("key1", "val1");
        cache.Set("key2", "val2");

        // Update key1, it should now be "newer" than key2
        cache.Set("key1", "val1_updated");

        // This should evict key2, not key1
        cache.Set("key3", "val3");

        Assert.Equal("val1_updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(maxSize: 10, ttlSeconds: 1);
        cache.Set("key1", "val1");

        Assert.Equal("val1", cache.Get("key1"));

        Thread.Sleep(1100);

        Assert.Null(cache.Get("key1"));
    }
}
