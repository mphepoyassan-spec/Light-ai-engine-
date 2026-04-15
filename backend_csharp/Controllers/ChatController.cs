using Microsoft.AspNetCore.Mvc;
using LightAI.Backend.Services;
using System.Text.Json;

namespace LightAI.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly ModelLoader _modelLoader;
    private readonly TokenSaver _tokenSaver;
    private readonly ResponseCache _cache;
    private readonly ConversationMemory _memory;

    public ChatController(ModelLoader modelLoader, TokenSaver tokenSaver, ResponseCache cache, ConversationMemory memory)
    {
        _modelLoader = modelLoader;
        _tokenSaver = tokenSaver;
        _cache = cache;
        _memory = memory;
    }

    [HttpPost]
    public IActionResult Chat([FromBody] ChatRequest request)
    {
        var cached = _cache.Get(request.Message);
        if (cached != null)
        {
            return Ok(new { response = cached, cached = true });
        }

        _memory.AddMessage(request.SessionId, "user", request.Message);

        var history = _memory.GetHistory(request.SessionId);
        var trimmed = _tokenSaver.TrimContext(history);
        var prompt = _tokenSaver.OptimizePrompt(trimmed);

        var response = _modelLoader.Predict(prompt);

        _memory.AddMessage(request.SessionId, "assistant", response);
        _cache.Set(request.Message, response);

        return Ok(new { response = response, cached = false });
    }

    [HttpPost("stream")]
    public async Task Stream([FromBody] ChatRequest request)
    {
        Response.Headers.Add("Content-Type", "text/event-stream");

        _memory.AddMessage(request.SessionId, "user", request.Message);
        var history = _memory.GetHistory(request.SessionId);
        var trimmed = _tokenSaver.TrimContext(history);
        var prompt = _tokenSaver.OptimizePrompt(trimmed);

        var fullResponse = _modelLoader.Predict(prompt);
        var words = fullResponse.Split(' ');

        foreach (var word in words)
        {
            var chunk = new { chunk = word + " ", done = false };
            await Response.WriteAsync($"data: {JsonSerializer.Serialize(chunk)}\n\n");
            await Response.Body.FlushAsync();
            await Task.Delay(50);
        }

        _memory.AddMessage(request.SessionId, "assistant", fullResponse);
        await Response.WriteAsync($"data: {JsonSerializer.Serialize(new { chunk = "", done = true })}\n\n");
        await Response.Body.FlushAsync();
    }

    [HttpPost("/clear")]
    public IActionResult Clear([FromBody] ChatRequest request)
    {
        _memory.ClearHistory(request.SessionId);
        return Ok(new { message = "History cleared", session_id = request.SessionId });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = "default";
}
