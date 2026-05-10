using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Cache_EvictsOldest_WhenMaxSizeExceeded()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("3", "three");

        Assert.Null(cache.Get("1"));
        Assert.Equal("two", cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public async Task Cache_EvictsExpired_Items()
    {
        var cache = new ResponseCache(10, 1); // 1 second TTL
        cache.Set("1", "one");
        Assert.Equal("one", cache.Get("1"));

        await Task.Delay(1100);

        Assert.Null(cache.Get("1"));
    }

    [Fact]
    public void Cache_HandlesUpdatedKeys_Correctly()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "one");
        cache.Set("2", "two");

        // Update "1", it should now be "newer" than "2"
        cache.Set("1", "one-updated");

        cache.Set("3", "three");

        // "2" should be evicted because it's the oldest now
        Assert.Null(cache.Get("2"));
        Assert.Equal("one-updated", cache.Get("1"));
        Assert.Equal("three", cache.Get("3"));
    }
}
