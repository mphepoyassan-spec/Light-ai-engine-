using System.Text.RegularExpressions;

namespace LightAI.Backend.Services;

public partial class TokenSaver
{
    private readonly int _maxContextMessages;
    private readonly string[] _fillerWords = { "uh", "um", "er", "ah", "like", "you know", "basically", "actually" };

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"([!?.,]){2,}", RegexOptions.Compiled)]
    private static partial Regex PunctuationRegex();

    [GeneratedRegex(@"^\W+", RegexOptions.Compiled)]
    private static partial Regex LeadingNonWordRegex();

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
        text = WhitespaceRegex().Replace(text, " ").Trim();

        // Normalize redundant punctuation
        text = PunctuationRegex().Replace(text, "$1");

        // Remove leading non-word characters
        text = LeadingNonWordRegex().Replace(text, "");

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
        var sb = new System.Text.StringBuilder();
        foreach (var msg in messages)
        {
            var content = CleanText(msg.Content);
            var role = msg.Role.ToLower() == "user" ? "User" : "AI";
            sb.Append($"{role}: {content}\n");
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
