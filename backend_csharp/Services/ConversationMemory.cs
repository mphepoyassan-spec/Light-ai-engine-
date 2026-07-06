using System.Collections.Concurrent;

namespace LightAI.Backend.Services;

public class ConversationMemory
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public List<ChatMessage> GetHistory(string sessionId)
    {
        var history = _conversations.GetOrAdd(sessionId, _ => new List<ChatMessage>());
        lock (history)
        {
            return new List<ChatMessage>(history);
        }
    }

    public void AddMessage(string sessionId, string role, string content)
    {
        var history = _conversations.GetOrAdd(sessionId, _ => new List<ChatMessage>());
        lock (history)
        {
            history.Add(new ChatMessage { Role = role, Content = content });
        }
    }

    public void ClearHistory(string sessionId)
    {
        _conversations.TryRemove(sessionId, out _);
    }
}
