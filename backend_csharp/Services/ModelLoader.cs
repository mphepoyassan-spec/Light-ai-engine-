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
            var userMsg = "your request";
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("User: "))
                {
                    userMsg = line.Substring(6).Trim();
                }
            }

            return $"I understand you're asking about: {userMsg}\n" +
                   $"As a local AI engine, I'm currently running in [Mock Mode] because the ONNX model is not loaded.\n" +
                   $"When the model is available at {_settings.ModelPath}, I will provide real-time inference.";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
