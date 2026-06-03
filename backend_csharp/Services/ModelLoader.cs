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
        // Extract the last user message from the prompt for a cleaner mock response
        var lastUserMessage = text;
        var lastUserIndex = text.LastIndexOf("User: ");
        if (lastUserIndex != -1)
        {
            var aiIndex = text.IndexOf("\nAI:", lastUserIndex);
            if (aiIndex != -1)
            {
                lastUserMessage = text.Substring(lastUserIndex + 6, aiIndex - (lastUserIndex + 6)).Trim();
            }
        }

        if (!_isReady || _session == null)
        {
            return $"I understand your request: \"{lastUserMessage}\". How can I assist you further? [Mock Mode]";
        }

        // Real inference logic would go here if we had the model signature
        // For now, even if session is loaded, we return a mock-like response with a note
        return $"Inference performed using model at {_settings.ModelPath} for input: \"{lastUserMessage}\". (Inference logic pending model signature)";
    }
}
