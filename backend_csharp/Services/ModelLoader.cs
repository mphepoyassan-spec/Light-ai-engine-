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
            var prefix = text.Length > 20 ? text.Substring(0, 20) + "..." : text;
            return $"I've analyzed your request starting with: '{prefix}'\nThis is a simulated response because the engine is running in [Mock Mode].";
        }

        // Real inference logic would go here if we had the model signature
        // For now, even if session is loaded, we return a mock-like response with a note
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
