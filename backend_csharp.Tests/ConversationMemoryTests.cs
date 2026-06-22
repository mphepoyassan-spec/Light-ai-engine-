using LightAI.Backend.Services;
using Xunit;

namespace LightAI.Tests;

public class ConversationMemoryTests
{
    [Fact]
    public void GetHistory_ReturnsSnapshot()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("s1", "user", "hi");

        var history1 = memory.GetHistory("s1");
        memory.AddMessage("s1", "user", "bye");
        var history2 = memory.GetHistory("s1");

        Assert.Single(history1);
        Assert.Equal(2, history2.Count);
    }

    [Fact]
    public void ClearHistory_Works()
    {
        var memory = new ConversationMemory();
        memory.AddMessage("s1", "user", "hi");
        memory.ClearHistory("s1");
        var history = memory.GetHistory("s1");
        Assert.Empty(history);
    }
}
