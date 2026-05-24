using LightAI.Backend.Controllers;
using LightAI.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LightAI.Tests;

public class PredictControllerTests
{
    private readonly PredictController _controller;
    private readonly Mock<IOptions<ModelSettings>> _mockSettings;

    public PredictControllerTests()
    {
        _mockSettings = new Mock<IOptions<ModelSettings>>();
        _mockSettings.Setup(s => s.Value).Returns(new ModelSettings { ModelPath = "mock.onnx" });
        var modelLoader = new ModelLoader(_mockSettings.Object);
        _controller = new PredictController(modelLoader);
    }

    [Fact]
    public void Predict_Returns400_WhenTextIsEmpty()
    {
        var request = new PredictRequest { Text = "" };
        var result = _controller.Predict(request);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Predict_ReturnsOk_WhenTextIsValid()
    {
        var request = new PredictRequest { Text = "test" };
        var result = _controller.Predict(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
