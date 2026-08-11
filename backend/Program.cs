using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MediatR;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using LoanApplication.Api.Infrastructure;
using LoanApplication.Api.Infrastructure.Messaging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión temporal para SQL Server (LocalDB). Cámbiala en producción.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");
builder.Services.AddDbContext<LoanDbContext>(options => options.UseSqlServer(connectionString));

// MediatR: registra handlers desde este ensamblado
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SubmitApplicationCommand).Assembly));

// Canal y publicador de eventos
builder.Services.AddSingleton<Channel<ApplicationSavedEvent>>(Channel.CreateUnbounded<ApplicationSavedEvent>());
builder.Services.AddSingleton<IBackgroundEventPublisher, ChannelEventPublisher>();

// Servicio en segundo plano que procesa eventos
builder.Services.AddHostedService<EventProcessingBackgroundService>();

// Cliente HTTP simulado para el background service
builder.Services.AddHttpClient("ExternalMock", client => client.BaseAddress = new Uri("http://localhost:5000"));

// Reglas de negocio simples (registradas como scoped)
builder.Services.AddScoped<ILoanRule, NyStateRule>();
builder.Services.AddScoped<ILoanRule, BlacklistedSsnRule>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LoanApplication.Api", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint minimal POST /api/applications
app.MapPost("/api/applications", async (IMediator mediator, SubmitApplicationCommand command) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
})
.WithName("SubmitApplication")
.Accepts<SubmitApplicationCommand>("application/json")
.Produces<SubmitApplicationResult>(StatusCodes.Status200OK);

app.Run();
