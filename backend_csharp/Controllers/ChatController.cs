using Microsoft.AspNetCore.Mvc;
using LightAI.Backend.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = true, message = "Message cannot be empty", code = 400 });
            }

            var cached = _cache.Get(request.Message);
            if (cached != null)
            {
                _memory.AddMessage(request.SessionId, "user", request.Message);
                _memory.AddMessage(request.SessionId, "assistant", cached);
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
        catch (Exception ex)
        {
            return StatusCode(500, new { error = true, message = ex.Message, code = 500 });
        }
    }

    [HttpPost("stream")]
    public async Task Stream([FromBody] ChatRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                Response.StatusCode = 400;
                await Response.WriteAsJsonAsync(new { error = true, message = "Message cannot be empty", code = 400 }, HttpContext.RequestAborted);
                return;
            }

            Response.Headers.Append("Content-Type", "text/event-stream");

            _memory.AddMessage(request.SessionId, "user", request.Message);
            var history = _memory.GetHistory(request.SessionId);
            var trimmed = _tokenSaver.TrimContext(history);
            var prompt = _tokenSaver.OptimizePrompt(trimmed);

            var fullResponse = _modelLoader.Predict(prompt);

            // Split while preserving whitespace
            var chunks = Regex.Split(fullResponse, @"(\s+)");

            foreach (var chunk in chunks)
            {
                if (string.IsNullOrEmpty(chunk)) continue;
                if (HttpContext.RequestAborted.IsCancellationRequested) break;

                var data = new { chunk = chunk, done = false };
                await Response.WriteAsync($"data: {JsonSerializer.Serialize(data)}\n\n", HttpContext.RequestAborted);
                await Response.Body.FlushAsync(HttpContext.RequestAborted);
                await Task.Delay(50, HttpContext.RequestAborted);
            }

            if (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                _memory.AddMessage(request.SessionId, "assistant", fullResponse);
                await Response.WriteAsync($"data: {JsonSerializer.Serialize(new { chunk = "", done = true })}\n\n", HttpContext.RequestAborted);
                await Response.Body.FlushAsync(HttpContext.RequestAborted);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected, nothing to do
        }
        catch (Exception ex)
        {
            if (!Response.HasStarted)
            {
                Response.StatusCode = 500;
                await Response.WriteAsJsonAsync(new { error = true, message = ex.Message, code = 500 });
            }
        }
    }

    [HttpPost("/clear")]
    public IActionResult Clear([FromBody] ChatRequest request)
    {
        try
        {
            _memory.ClearHistory(request.SessionId);
            return Ok(new { message = "History cleared", session_id = request.SessionId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = true, message = ex.Message, code = 500 });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = "default";
}
