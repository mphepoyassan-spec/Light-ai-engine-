using System.Text;
using System.Text.RegularExpressions;

namespace LightAI.Backend.Services;

public class TokenSaver
{
    private readonly int _maxContextMessages;
    private readonly string[] _fillerWords = { "uh", "um", "er", "ah", "like", "you know", "basically", "actually" };

    public TokenSaver(int maxContextMessages = 5)
    {
        _maxContextMessages = maxContextMessages;
    }

    public string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // Remove filler words
        foreach (var word in _fillerWords)
        {
            text = Regex.Replace(text, $@"\s*\b{word}\b\s*", " ", RegexOptions.IgnoreCase);
        }

        // Normalize whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();

        // Normalize redundant punctuation
        text = Regex.Replace(text, @"([!?.,]){2,}", "$1");

        return text.Trim();
    }

    public List<T> TrimContext<T>(List<T> messages)
    {
        if (messages.Count > _maxContextMessages)
        {
            return messages.Skip(messages.Count - _maxContextMessages).ToList();
        }
        return messages;
    }

    public string OptimizePrompt(List<ChatMessage> messages)
    {
        var sb = new StringBuilder();
        foreach (var msg in messages)
        {
            var content = CleanText(msg.Content);
            var role = msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? "User" : "AI";
            sb.Append(role).Append(": ").Append(content).Append('\n');
        }
        sb.Append("AI:");
        return sb.ToString();
    }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
