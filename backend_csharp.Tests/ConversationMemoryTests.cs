using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ConversationMemoryTests
{
    [Fact]
    public void AddMessage_And_GetHistory_Works()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("s1", "user", "hello");

        var history = memory.GetHistory("s1");
        Assert.Single(history);
        Assert.Equal("hello", history[0].Content);
    }

    [Fact]
    public void ClearHistory_Works()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("s1", "user", "hello");
        memory.ClearHistory("s1");

        var history = memory.GetHistory("s1");
        Assert.Empty(history);
    }

    [Fact]
    public void GetHistory_ReturnsCopy()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("s1", "user", "hello");

        var history1 = memory.GetHistory("s1");
        memory.AddMessage("s1", "assistant", "hi");

        var history2 = memory.GetHistory("s1");

        Assert.Single(history1);
        Assert.Equal(2, history2.Count);
    }
}
