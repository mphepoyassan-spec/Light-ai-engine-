using System.Text;
using System.Text.RegularExpressions;

namespace LightAI.Backend.Services;

public class TokenSaver
{
    private readonly int _maxContextMessages;
    private static readonly string[] _fillerWords = { "uh", "um", "er", "ah", "like", "you know", "basically", "actually" };

    private static readonly Regex _fillerRegex = new Regex($@"\s*\b({string.Join("|", _fillerWords)})\b\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _whitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
    private static readonly Regex _punctuationRegex = new Regex(@"([!?.,]){2,}", RegexOptions.Compiled);
    private static readonly Regex _leadingNonWordRegex = new Regex(@"^\W+", RegexOptions.Compiled);

    public TokenSaver(int maxContextMessages = 5)
    {
        _maxContextMessages = maxContextMessages;
    }

    public string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // Remove filler words
        text = _fillerRegex.Replace(text, " ");

        // Normalize whitespace
        text = _whitespaceRegex.Replace(text, " ").Trim();

        // Normalize redundant punctuation
        text = _punctuationRegex.Replace(text, "$1");

        // Remove leading non-word characters
        text = _leadingNonWordRegex.Replace(text, "");

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
            var role = msg.Role.ToLower() == "user" ? "User" : "AI";
            sb.Append(role).Append(": ").Append(content).Append("\n");
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
