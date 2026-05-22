using System.Text;
using System.Text.RegularExpressions;

namespace LightAI.Backend.Services;

public partial class TokenSaver
{
    private readonly int _maxContextMessages;

    [GeneratedRegex(@"\s*\b(uh|um|er|ah|like|you know|basically|actually)\b\s*", RegexOptions.IgnoreCase)]
    private static partial Regex FillerWordsRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"([!?.,]){2,}")]
    private static partial Regex RedundantPunctuationRegex();

    [GeneratedRegex(@"^\W+")]
    private static partial Regex LeadingNonWordRegex();

    public TokenSaver(int maxContextMessages = 5)
    {
        _maxContextMessages = maxContextMessages;
    }

    public string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        text = FillerWordsRegex().Replace(text, " ");
        text = WhitespaceRegex().Replace(text, " ");
        text = RedundantPunctuationRegex().Replace(text, "$1");
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
