namespace LightAI.Backend.Services;

public class ModelSettings
{
    public string ModelPath { get; set; } = string.Empty;
    public int MaxContextMessages { get; set; } = 5;
    public int CacheTTLSeconds { get; set; } = 300;
}
