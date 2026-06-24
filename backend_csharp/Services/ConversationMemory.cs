using System.Collections.Concurrent;

namespace LightAI.Backend.Services;

public class ConversationMemory
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public List<ChatMessage> GetHistory(string sessionId)
    {
        var history = GetOrCreateHistory(sessionId);
        lock (history)
        {
            return history.ToList();
        }
    }

    public void AddMessage(string sessionId, string role, string content)
    {
        var history = GetOrCreateHistory(sessionId);
        lock (history)
        {
            history.Add(new ChatMessage { Role = role, Content = content });
        }
    }

    private List<ChatMessage> GetOrCreateHistory(string sessionId)
    {
        return _conversations.GetOrAdd(sessionId, _ => new List<ChatMessage>());
    }

    public void ClearHistory(string sessionId)
    {
        _conversations.TryRemove(sessionId, out _);
    }
}
