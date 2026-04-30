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
            // Extract the last user message from the formatted prompt
            var lines = prompt.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var lastUserMessage = lines.LastOrDefault(l => l.StartsWith("User: "))?.Substring(6) ?? "your message";

            return $"I have processed your request: \"{lastUserMessage}\".\n" +
                   "Currently running in [Mock Mode] as the ONNX model is not available.\n" +
                   "In a production environment, this would be the output of the local LLM.";
        }

        // Real inference logic would go here if we had the model signature
        return $"Inference performed using model at {_settings.ModelPath}.\n" +
               "This is a placeholder for the actual model output based on the provided prompt.";
    }
}
