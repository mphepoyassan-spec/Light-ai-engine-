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
            new ChatMessage { Role = "user", Content = "1" },
            new ChatMessage { Role = "user", Content = "2" },
            new ChatMessage { Role = "user", Content = "3" }
        };
        var result = ts.TrimContext(messages);
        Assert.Equal(2, result.Count);
        Assert.Equal("2", result[0].Content);
    }

    [Fact]
    public void OptimizePrompt_UsesStringBuilder_And_CorrectRoles()
    {
        var ts = new TokenSaver();
        var messages = new List<ChatMessage>
        {
            new ChatMessage { Role = "user", Content = "hello" },
            new ChatMessage { Role = "assistant", Content = "hi" }
        };
        var result = ts.OptimizePrompt(messages);
        Assert.Equal("User: hello\nAI: hi\nAI:", result);
    }
}
