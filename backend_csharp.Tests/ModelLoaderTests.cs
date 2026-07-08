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
    public void Predict_MockMode_ExtractsLastUserMessage()
    {
        var settings = new ModelSettings { ModelPath = "non_existent.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);
        loader.Load();

        var prompt = "User: Hello\nAI: Hi\nUser: How are you?\nAI:";
        var result = loader.Predict(prompt);

        Assert.Contains("I understand you said: 'How are you?'", result);
    }
}
