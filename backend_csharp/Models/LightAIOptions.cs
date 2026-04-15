namespace LightAI.Backend.Models;

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class LightAIOptions
{
    public int MaxContextMessages { get; set; } = 5;
    public int CacheTTLSeconds { get; set; } = 300;
    public string ModelPath { get; set; } = "backend/model.onnx";
    public string TokenizerName { get; set; } = "gpt2";
}
