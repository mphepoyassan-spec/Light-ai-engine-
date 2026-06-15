using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;

namespace LightAI.Backend.Services;

public class ModelLoader
{
    private readonly ModelSettings _settings;
    private bool _isReady = false;
    private InferenceSession? _session;

    public ModelLoader(IOptions<ModelSettings> settings)
    {
        _settings = settings.Value;
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_settings.ModelPath))
            {
                _session = new InferenceSession(_settings.ModelPath);
                _isReady = true;
                Console.WriteLine($"Model loaded successfully from {_settings.ModelPath}");
            }
            else
            {
                Console.WriteLine($"Model file not found at {_settings.ModelPath}. Falling back to mock mode.");
                _isReady = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading model: {ex.Message}. Falling back to mock mode.");
            _isReady = false;
        }
    }

    public string Predict(string prompt)
    {
        if (!_isReady || _session == null)
        {
            // Extract the last user message from the prompt for a better mock response
            var lines = prompt.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var lastUserLine = lines.LastOrDefault(l => l.StartsWith("User: "));
            var userMessage = lastUserLine?.Substring(6) ?? "your request";

            return $"I understand you're asking about '{userMessage}'. [Mock Mode]";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed for prompt: {prompt.Substring(0, Math.Min(20, prompt.Length))}... (Inference logic pending model signature)";
    }
}
