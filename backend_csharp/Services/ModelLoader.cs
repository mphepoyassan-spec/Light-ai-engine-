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
            // Try to extract the last user message from the prompt for a better mock response
            var lastUserMessage = "";
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].StartsWith("User: "))
                {
                    lastUserMessage = lines[i].Substring(6).Trim();
                    break;
                }
            }

            if (!string.IsNullOrEmpty(lastUserMessage))
            {
                return $"I understand you're asking about \"{lastUserMessage}\". [Mock Mode]";
            }
            return "I understand your request. [Mock Mode]";
        }

        // Real inference logic would go here if we had the model signature
        // For now, even if session is loaded, we return a mock-like response with a note
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
