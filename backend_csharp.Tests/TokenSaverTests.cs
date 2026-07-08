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
    public void CleanText_PreservesLeadingPunctuation()
    {
        var ts = new TokenSaver();
        Assert.Equal("!hello", ts.CleanText("!hello"));
        Assert.Equal("/help", ts.CleanText("/help"));
    }

    [Fact]
    public void CleanText_PreservesNewlines()
    {
        var ts = new TokenSaver();
        var input = "Line 1\nLine 2";
        var result = ts.CleanText(input);
        Assert.Contains("\n", result);
        Assert.Equal("Line 1\nLine 2", result);
    }
}
