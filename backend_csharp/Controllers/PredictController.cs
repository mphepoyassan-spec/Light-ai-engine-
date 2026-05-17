using Microsoft.AspNetCore.Mvc;
using LightAI.Backend.Services;

namespace LightAI.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class PredictController : ControllerBase
{
    private readonly ModelLoader _modelLoader;

    public PredictController(ModelLoader modelLoader)
    {
        _modelLoader = modelLoader;
    }

    [HttpPost]
    public IActionResult Predict([FromBody] PredictRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { error = true, message = "Text cannot be empty", code = 400 });
        }

        var response = _modelLoader.Predict(request.Text);
        return Ok(new { prediction = response });
    }
}

public class PredictRequest
{
    public string Text { get; set; } = string.Empty;
}
