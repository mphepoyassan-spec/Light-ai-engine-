using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using System.Text.RegularExpressions;

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

    public string Predict(string text)
    {
        if (!_isReady || _session == null)
        {
            // Simple logic to make mock mode more context-aware
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var lastUserMessage = lines.LastOrDefault(l => l.StartsWith("User:"))?.Replace("User:", "").Trim();

            if (!string.IsNullOrEmpty(lastUserMessage))
            {
                return $"I understand you are asking about '{lastUserMessage}'. [Mock Mode]";
            }
            return "I understand your request. [Mock Mode]";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
