using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class TokenSaverTests
{
    [Fact]
    public void CleanText_RemovesFillerWords()
    {
        var ts = new TokenSaver();
        var result = ts.CleanText("Hello um world");
        Assert.Equal("Hello world", result);
    }

    [Fact]
    public void CleanText_NormalizesPunctuation()
    {
        var ts = new TokenSaver();
        var result = ts.CleanText("Hello... world!!");
        Assert.Equal("Hello. world!", result);
    }

    [Fact]
    public void TrimContext_Works()
    {
        var ts = new TokenSaver(2);
        var messages = new List<ChatMessage>
        {
            new ChatMessage { Content = "1" },
            new ChatMessage { Content = "2" },
            new ChatMessage { Content = "3" }
        };
        var result = ts.TrimContext(messages);
        Assert.Equal(2, result.Count);
        Assert.Equal("2", result[0].Content);
    }

    [Fact]
    public void CleanText_PreservesCommandPrefixes()
    {
        var ts = new TokenSaver();
        Assert.Equal("!help", ts.CleanText("!help"));
        Assert.Equal("/clear", ts.CleanText("/clear"));
    }
}
