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

        // Normalize whitespace (preserving single newlines)
        text = Regex.Replace(text, @"[^\S\r\n]+", " ");
        text = Regex.Replace(text, @"\n{2,}", "\n");

        // Normalize redundant punctuation
        text = Regex.Replace(text, @"([!?.,]){2,}", "$1");

        // Remove leading non-word characters except ! and /
        text = Regex.Replace(text, @"^[^\w!/]+", "");

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
        var prompt = "";
        foreach (var msg in messages)
        {
            var content = CleanText(msg.Content);
            var role = msg.Role.ToLower() == "user" ? "User" : "AI";
            prompt += $"{role}: {content}\n";
        }
        prompt += "AI:";
        return prompt;
    }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
