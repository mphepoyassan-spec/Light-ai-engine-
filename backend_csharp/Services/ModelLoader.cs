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
            // Try to extract the last user message from the prompt
            var lines = prompt.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var lastUserLine = lines.LastOrDefault(l => l.StartsWith("User: "));
            var userMessage = lastUserLine != null ? lastUserLine.Substring(6) : "your request";

            return $"I understand you said: \"{userMessage}\"\n\nThis is a simulated response because the local AI model is currently in [Mock Mode].\n\nTo enable real inference, please ensure a valid ONNX model is placed at: {_settings.ModelPath}";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
