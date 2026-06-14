using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_And_Get_Works()
    {
        var cache = new ResponseCache(10, 100);
        cache.Set("key1", "value1");
        Assert.Equal("value1", cache.Get("key1"));
    }

    [Fact]
    public void Eviction_Works_FIFO()
    {
        var cache = new ResponseCache(2, 100);
        cache.Set("key1", "value1");
        cache.Set("key2", "value2");
        cache.Set("key3", "value3");

        Assert.Null(cache.Get("key1"));
        Assert.Equal("value2", cache.Get("key2"));
        Assert.Equal("value3", cache.Get("key3"));
    }

    [Fact]
    public void TTL_Works()
    {
        var cache = new ResponseCache(10, 0); // 0 seconds TTL
        cache.Set("key1", "value1");
        Thread.Sleep(10); // Small sleep to ensure time pass
        Assert.Null(cache.Get("key1"));
    }

    [Fact]
    public void Clear_Works()
    {
        var cache = new ResponseCache(10, 100);
        cache.Set("key1", "value1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));
    }
}
