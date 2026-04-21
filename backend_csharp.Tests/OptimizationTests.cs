using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class TokenSaverExtraTests
{
    [Fact]
    public void CleanText_HandlesLargePunctuation()
    {
        var ts = new TokenSaver();
        var result = ts.CleanText("Wait!!!! What????");
        Assert.Equal("Wait! What?", result);
    }

    [Fact]
    public void OptimizePrompt_UsesCorrectFormat()
    {
        var ts = new TokenSaver();
        var messages = new List<ChatMessage>
        {
            new ChatMessage { Role = "user", Content = "Hello" },
            new ChatMessage { Role = "assistant", Content = "Hi there" }
        };
        var result = ts.OptimizePrompt(messages);
        Assert.Equal("User: Hello\nAI: Hi there\nAI:", result);
    }
}

public class ResponseCacheTests
{
    [Fact]
    public void Set_EvictsOldest_WhenFull()
    {
        var cache = new ResponseCache(2, 60);
        cache.Set("1", "v1");
        cache.Set("2", "v2");
        cache.Set("3", "v3");

        Assert.Null(cache.Get("1"));
        Assert.Equal("v2", cache.Get("2"));
        Assert.Equal("v3", cache.Get("3"));
    }
}
