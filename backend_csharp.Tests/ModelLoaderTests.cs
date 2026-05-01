using LightAI.Backend.Services;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

namespace LightAI.Tests;

public class ModelLoaderTests
{
    [Fact]
    public void Predict_ReturnsImprovedMockMessage_WhenNotReady()
    {
        var settings = new ModelSettings { ModelPath = "non_existent.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);

        loader.Load();
        var prompt = "User: What is AI?\nAI:";
        var result = loader.Predict(prompt);

        Assert.Contains("[Mock Mode]", result);
        Assert.Contains("What is AI?", result);
    }
}
