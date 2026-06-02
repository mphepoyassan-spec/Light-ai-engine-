using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void SetAndGet_ReturnsValue()
    {
        var cache = new ResponseCache();
        cache.Set("key1", "value1");
        Assert.Equal("value1", cache.Get("key1"));
    }

    [Fact]
    public void Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(100, 1); // 1 second TTL
        cache.Set("key1", "value1");
        Thread.Sleep(1100);
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Set_EvictsOldest_WhenFull()
    {
        var cache = new ResponseCache(2, 300); // Max size 2
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3");

        Assert.Null(cache.Get("key1")); // Should be evicted (FIFO)
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void Clear_RemovesAll()
    {
        var cache = new ResponseCache();
        cache.Set("key1", "value1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));
    }
}
