using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenMaxSizeReached()
    {
        var cache = new ResponseCache(2, 300);
        cache.Set("1", "one");
        cache.Set("2", "two");
        cache.Set("3", "three"); // Should evict "1"

        Assert.Null(cache.Get("1"));
        Assert.Equal("two", cache.Get("2"));
        Assert.Equal("three", cache.Get("3"));
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var cache = new ResponseCache(10, 300);
        cache.Set("1", "one");
        cache.Clear();

        Assert.Null(cache.Get("1"));

        // Ensure keys queue is also cleared by checking eviction logic after clear
        cache.Set("2", "two");
        cache.Set("3", "three");
        cache.Set("4", "four");

        var smallCache = new ResponseCache(2, 300);
        smallCache.Set("1", "one");
        smallCache.Clear();
        smallCache.Set("2", "two");
        smallCache.Set("3", "three");
        smallCache.Set("4", "four"); // Should evict "2" if queue was cleared correctly

        Assert.Null(smallCache.Get("2"));
    }
}
