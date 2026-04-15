using LightAI.Backend.Models;
using LightAI.Backend.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<LightAIOptions>(builder.Configuration.GetSection("LightAI"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ModelLoader>();
builder.Services.AddSingleton<TokenSaver>();
builder.Services.AddSingleton<ResponseCache>();
builder.Services.AddSingleton<ConversationMemory>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();

// Initial loading
var modelLoader = app.Services.GetRequiredService<ModelLoader>();
modelLoader.Load();

app.MapControllers();

// Serve frontend files
var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "frontend");
if (!Directory.Exists(frontendPath))
{
    frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend");
}

if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath),
        RequestPath = "/frontend"
    });
}

// Root endpoint
app.MapGet("/", () => Results.Ok(new { status = "online", engine = "LightAI", lang = "C#" }));

app.Run();
