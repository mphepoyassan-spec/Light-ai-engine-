using LightAI.Backend.Services;
using LightAI.Backend.Models;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LightAI.Tests;

public class TokenSaverTests
{
    private IOptions<LightAIOptions> CreateOptions(int maxMessages = 5)
    {
        var options = new LightAIOptions { MaxContextMessages = maxMessages };
        var mock = new Mock<IOptions<LightAIOptions>>();
        mock.Setup(m => m.Value).Returns(options);
        return mock.Object;
    }

    [Fact]
    public void CleanText_RemovesFillerWords()
    {
        var ts = new TokenSaver(CreateOptions());
        var result = ts.CleanText("Hello um world");
        Assert.Equal("Hello world", result);
    }

    [Fact]
    public void CleanText_NormalizesPunctuation()
    {
        var ts = new TokenSaver(CreateOptions());
        var result = ts.CleanText("Hello... world!!");
        Assert.Equal("Hello. world!", result);
    }

    [Fact]
    public void TrimContext_Works()
    {
        var ts = new TokenSaver(CreateOptions(2));
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
}
