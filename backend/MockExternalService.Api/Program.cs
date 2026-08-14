using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5001
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001);
});

var app = builder.Build();

app.MapPost("/api/external-mock", (ILogger<Program> logger, JsonElement payload) =>
{
    var json = JsonSerializer.Serialize(payload);
    logger.LogInformation("Received external payload: {Payload}", json);
    return Results.Ok();
});

app.Run();
