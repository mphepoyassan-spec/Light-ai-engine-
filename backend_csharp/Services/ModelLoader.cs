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
            // Try to extract the user's specific message from the prompt
            var lines = prompt.Split('\n');
            var lastUserLine = lines.LastOrDefault(l => l.StartsWith("User: "));
            var userMsg = lastUserLine?.Substring(6) ?? "your message";

            return $"[Mock Mode]\nI understand you are asking about: \"{userMsg}\".\nSince I am running in mock mode, I can't generate a real AI response, but I can confirm that the system is correctly processing your input through the full pipeline.";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
