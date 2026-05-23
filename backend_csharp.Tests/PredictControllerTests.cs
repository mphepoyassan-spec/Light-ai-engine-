using LightAI.Backend.Controllers;
using LightAI.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace LightAI.Tests;

public class PredictControllerTests
{
    [Fact]
    public void Predict_ReturnsBadRequest_WhenTextIsEmpty()
    {
        var settings = new ModelSettings { ModelPath = "mock.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);
        var controller = new PredictController(loader);

        var request = new PredictRequest { Text = "" };
        var result = controller.Predict(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public void Predict_ReturnsOk_WhenTextIsProvided()
    {
        var settings = new ModelSettings { ModelPath = "mock.onnx" };
        var options = Options.Create(settings);
        var loader = new ModelLoader(options);
        var controller = new PredictController(loader);

        var request = new PredictRequest { Text = "hello" };
        var result = controller.Predict(request);

        Assert.IsType<OkObjectResult>(result);
    }
}
