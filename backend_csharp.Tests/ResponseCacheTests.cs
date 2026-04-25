using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3"); // Should evict key1

        Assert.Null(cache.Get("key1"));
        Assert.Equal("val2", cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }

    [Fact]
    public void Set_UpdatesValue_WithoutDoubleEnqueue()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key1", "val1_updated");
        cache.Set("key3", "val3"); // Should evict key2, NOT key1

        Assert.Equal("val1_updated", cache.Get("key1"));
        Assert.Null(cache.Get("key2"));
        Assert.Equal("val3", cache.Get("key3"));
    }
}
