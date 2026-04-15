namespace LightAI.Backend.Services;

public class ModelLoader
{
    private bool _isReady = false;

    public void Load()
    {
        // Placeholder for ONNX runtime loading
        _isReady = false; // Stay in mock mode for now
    }

    public string Predict(string text)
    {
        if (!_isReady)
        {
            return "I understand your request. [Mock Mode]";
        }

        // Inference logic would go here
        return "Inference not implemented in mock mode.";
    }
}
