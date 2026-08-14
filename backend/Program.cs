using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MediatR;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using LoanApplication.Api.Infrastructure.Persistence;
using LoanApplication.Api.Infrastructure.Messaging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Force the app to listen on http://localhost:5000 so frontend can reach it
builder.WebHost.UseUrls("http://localhost:5000");

// Registrar DbContext: usar cadena de conexión si existe, si no usar InMemory para facilitar ejecución local/tests
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<LoanDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<LoanDbContext>(options => options.UseInMemoryDatabase("LoanDb"));
}

// MediatR: registra handlers desde este ensamblado (Program assembly)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// Registrar validation behavior pipeline
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoanApplication.Api.Application.Rules.ValidationBehavior<,>));

// Register FluentValidation validators
builder.Services.AddTransient<FluentValidation.IValidator<LoanApplication.Api.Application.Features.Loans.SubmitApplicationCommand>, LoanApplication.Api.Application.Rules.SubmitApplicationCommandValidator>();

// Canal y publicador de eventos
builder.Services.AddSingleton(Channel.CreateUnbounded<ApplicationSavedEvent>());
builder.Services.AddTransient<IBackgroundEventPublisher, ChannelEventPublisher>();

// Servicio en segundo plano que procesa eventos
builder.Services.AddHostedService<EventProcessingBackgroundService>();

// Cliente HTTP simulado para el background service
builder.Services.AddHttpClient("ExternalMock", client => client.BaseAddress = new Uri("http://localhost:5001"));

// Permitir CORS para el frontend Next.js en localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader());
});

// Reglas de negocio registradas como transient (exacto solicitado)
builder.Services.AddTransient<ILoanRule, NyStateRule>();
builder.Services.AddTransient<ILoanRule, BlacklistedSsnRule>();
// Nuevas reglas solicitadas
builder.Services.AddTransient<ILoanRule, MaxRequestedAmountRule>();
builder.Services.AddTransient<ILoanRule, MinRequestedAmountRule>();
builder.Services.AddTransient<ILoanRule, SsnFormatRule>();

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

// Global exception handling middleware (must be before other middleware so it can catch exceptions)
app.UseMiddleware<LoanApplication.Api.Infrastructure.ExceptionHandlingMiddleware>();

// Habilitar CORS para el frontend (Next.js)
app.UseCors("AllowFrontend");

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
