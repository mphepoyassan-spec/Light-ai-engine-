using LightAI.Backend.Controllers;
using LightAI.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

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

        var result = controller.Predict(new PredictRequest { Text = "" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
    }
}
