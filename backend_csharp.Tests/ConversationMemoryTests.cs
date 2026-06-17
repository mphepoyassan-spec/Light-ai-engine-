using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ConversationMemoryTests
{
    [Fact]
    public void AddAndGetHistory_Works()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("session1", "user", "hello");

        var history = memory.GetHistory("session1");

        Assert.Single(history);
        Assert.Equal("hello", history[0].Content);
    }

    [Fact]
    public void GetHistory_ReturnsSnapshot()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("session1", "user", "hello");

        var history = memory.GetHistory("session1");
        memory.AddMessage("session1", "assistant", "hi");

        Assert.Single(history);
        var history2 = memory.GetHistory("session1");
        Assert.Equal(2, history2.Count);
    }
}
