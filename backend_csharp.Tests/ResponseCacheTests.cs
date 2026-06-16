using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Set("key2", "val2");
        cache.Set("key3", "val3");

        Assert.Null(cache.Get("key1"));
        Assert.NotNull(cache.Get("key2"));
        Assert.NotNull(cache.Get("key3"));
    }

    [Fact]
    public void Clear_FlushesEverything()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("key1", "val1");
        cache.Clear();
        Assert.Null(cache.Get("key1"));
    }
}
