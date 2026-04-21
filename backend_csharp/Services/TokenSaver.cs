using System.Text;
using System.Text.RegularExpressions;

namespace LightAI.Backend.Services;

public class TokenSaver
{
    private readonly int _maxContextMessages;
    private static readonly string[] _fillerWords = { "uh", "um", "er", "ah", "like", "you know", "basically", "actually" };

    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex PunctuationRegex = new(@"([!?.,]){2,}", RegexOptions.Compiled);
    private static readonly Regex LeadingNonWordRegex = new(@"^\W+", RegexOptions.Compiled);
    private static readonly Regex FillerRegex;

    static TokenSaver()
    {
        var fillerPattern = $@"\s*\b({string.Join("|", _fillerWords)})\b\s*";
        FillerRegex = new Regex(fillerPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public TokenSaver(int maxContextMessages = 5)
    {
        _maxContextMessages = maxContextMessages;
    }

    public string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // Remove filler words
        text = FillerRegex.Replace(text, " ");

        // Normalize whitespace
        text = WhitespaceRegex.Replace(text, " ").Trim();

        // Normalize redundant punctuation
        text = PunctuationRegex.Replace(text, "$1");

        // Remove leading non-word characters
        text = LeadingNonWordRegex.Replace(text, "");

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
