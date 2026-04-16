using LightAI.Backend.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ModelSettings>(builder.Configuration.GetSection("ModelSettings"));

builder.Services.AddControllers();
builder.Services.AddSingleton<ModelLoader>();
builder.Services.AddSingleton<TokenSaver>(sp =>
{
    var settings = builder.Configuration.GetSection("ModelSettings").Get<ModelSettings>();
    return new TokenSaver(settings?.MaxContextMessages ?? 5);
});
builder.Services.AddSingleton<ResponseCache>(sp =>
{
    var settings = builder.Configuration.GetSection("ModelSettings").Get<ModelSettings>();
    return new ResponseCache(100, settings?.CacheTTLSeconds ?? 300);
});
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
// When running with dotnet run --project backend_csharp, CurrentDirectory is /app
var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "frontend");
if (!Directory.Exists(frontendPath))
{
    // Fallback for different working directories
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

app.Run("http://0.0.0.0:8000");
