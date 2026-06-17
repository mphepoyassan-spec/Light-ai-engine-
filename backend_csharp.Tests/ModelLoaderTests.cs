using LightAI.Backend.Services;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

namespace LightAI.Tests;

public class ModelLoaderTests
{
    [Fact]
    public void Predict_ReturnsMockMessage_WhenNotReady()
    {
        var settings = new ModelSettings { ModelPath = "non_existent.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);

        loader.Load();
        var result = loader.Predict("hello");

        Assert.Contains("[Mock Mode]", result);
    }

    [Fact]
    public void Predict_ReturnsContextAwareMockMessage()
    {
        var settings = new ModelSettings { ModelPath = "non_existent.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);

        loader.Load();
        var result = loader.Predict("User: What is the weather?\nAI: I don't know.\nUser: Tell me a joke.\nAI:");

        Assert.Contains("Tell me a joke", result);
        Assert.Contains("[Mock Mode]", result);
    }
}
