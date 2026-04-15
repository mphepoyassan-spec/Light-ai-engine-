using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using LightAI.Backend.Models;

namespace LightAI.Backend.Services;

public class TokenSaver
{
    private readonly int _maxContextMessages;
    private readonly string[] _fillerWords = { "uh", "um", "er", "ah", "like", "you know", "basically", "actually" };

    public TokenSaver(IOptions<LightAIOptions> options)
    {
        _maxContextMessages = options.Value.MaxContextMessages;
    }

    public string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        foreach (var word in _fillerWords)
        {
            text = Regex.Replace(text, $@"\s*\b{word}\b\s*", " ", RegexOptions.IgnoreCase);
        }

        text = Regex.Replace(text, @"\s+", " ").Trim();
        text = Regex.Replace(text, @"([!?.,]){2,}", "$1");
        text = Regex.Replace(text, @"^\W+", "");

        return text.Trim();
    }

    public List<ChatMessage> TrimContext(List<ChatMessage> messages)
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
