using LightAI.Backend.Services;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

namespace LightAI.Tests;

public class ModelLoaderTests
{
    [Fact]
    public void Predict_ReturnsContextAwareMockMessage_WhenNotReady()
    {
        var settings = new ModelSettings { ModelPath = "non_existent.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);

        loader.Load();
        var result = loader.Predict("User: How are you?\nAI:");

        Assert.Contains("How are you?", result);
        Assert.Contains("[Mock Mode]", result);
    }
}
