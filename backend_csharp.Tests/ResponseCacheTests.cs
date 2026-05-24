using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_RespectsMaxSize()
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
    public void Set_UpdatesKey_DoesNotEvictIncorrectly()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("1", "one-updated"); // This should enqueue "1" again with new timestamp
        cache.Set("3", "three"); // This should evict "2", not "1"

        Assert.Equal("one-updated", cache.Get("1"));
        Assert.Null(cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public void Get_ReturnsNull_AfterTTL()
    {
        var cache = new ResponseCache(100, 0); // 0 seconds TTL
        cache.Set("1", "one");
        // Even with 0 seconds, DateTime.UtcNow might be same if fast, but typically it will expire
        // Wait a bit to be sure
        Thread.Sleep(10);
        Assert.Null(cache.Get("1"));
    }
}
