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
    public void OptimizePrompt_UsesStringBuilder_And_ReturnsCorrectFormat()
    {
        var ts = new TokenSaver();
        var messages = new List<ChatMessage>
        {
            new ChatMessage { Role = "user", Content = "Hello" },
            new ChatMessage { Role = "assistant", Content = "Hi" }
        };
        var result = ts.OptimizePrompt(messages);
        var expected = "User: Hello\nAI: Hi\nAI:";
        Assert.Equal(expected, result);
    }
}
