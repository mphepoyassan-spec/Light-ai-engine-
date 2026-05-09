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
            // Extract the last user message from the prompt
            var lines = prompt.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var userMessage = "";
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].StartsWith("User: "))
                {
                    userMessage = lines[i].Substring(6);
                    break;
                }
            }

            return $"I understand your request about: \"{userMessage}\".\n\n" +
                   $"Note: The system is currently in [Mock Mode] because the model file at '{_settings.ModelPath}' was not found or failed to load.\n" +
                   "Please ensure the ONNX model is correctly placed to enable real inference.";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}. (Inference logic pending model signature)";
    }
}
