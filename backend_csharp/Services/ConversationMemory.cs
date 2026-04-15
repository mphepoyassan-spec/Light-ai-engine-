using System.Collections.Concurrent;
using LightAI.Backend.Models;

namespace LightAI.Backend.Services;

public class ConversationMemory
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public List<ChatMessage> GetHistory(string sessionId)
    {
        return _conversations.GetOrAdd(sessionId, _ => new List<ChatMessage>());
    }

    public void AddMessage(string sessionId, string role, string content)
    {
        var history = GetHistory(sessionId);
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
