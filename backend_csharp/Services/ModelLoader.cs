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

    public string Predict(string text)
    {
        if (!_isReady || _session == null)
        {
            var userMessage = ExtractUserMessage(text);
            return $"I understand your request regarding: \"{userMessage}\".\n\n[Mock Mode]\nThis is a simulated response because the actual ONNX model was not found or failed to load.";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }

    private string ExtractUserMessage(string prompt)
    {
        var lines = prompt.TrimEnd().Split('\n');
        foreach (var line in lines.Reverse())
        {
            if (line.StartsWith("User: ", StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(6).Trim();
            }
        }
        return "Unknown";
    }
}
