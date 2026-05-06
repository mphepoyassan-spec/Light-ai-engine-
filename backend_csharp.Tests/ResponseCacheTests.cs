using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_RespectsMaxSize_FIFO()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("3", "three");

        Assert.Null(cache.Get("1"));
        Assert.Equal("two", cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public void Set_UpdateKey_HandlesEvictionCorrectly()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "one");
        cache.Set("2", "two");

        // Update "1", it should now be considered "newer" in terms of timestamp
        cache.Set("1", "one-updated");

        // Add "3", "2" should be evicted because it's the oldest now
        cache.Set("3", "three");

        Assert.Equal("one-updated", cache.Get("1"));
        Assert.Null(cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }
}
