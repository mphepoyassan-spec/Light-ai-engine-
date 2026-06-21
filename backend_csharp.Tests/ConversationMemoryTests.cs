using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ConversationMemoryTests
{
    [Fact]
    public void GetHistory_ReturnsSnapshot()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("session1", "user", "hello");

        var history1 = memory.GetHistory("session1");
        memory.AddMessage("session1", "assistant", "hi");
        var history2 = memory.GetHistory("session1");

        Assert.Single(history1);
        Assert.Equal(2, history2.Count);
    }

    [Fact]
    public void ClearHistory_Works()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("session1", "user", "hello");
        memory.ClearHistory("session1");
        Assert.Empty(memory.GetHistory("session1"));
    }
}
