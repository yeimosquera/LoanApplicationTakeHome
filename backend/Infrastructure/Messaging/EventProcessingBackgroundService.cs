using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoanApplication.Api.Infrastructure.Messaging;

public sealed class EventProcessingBackgroundService : BackgroundService
{
    private readonly Channel<ApplicationSavedEvent> _channel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EventProcessingBackgroundService> _logger;

    public EventProcessingBackgroundService(
        Channel<ApplicationSavedEvent> channel,
        IHttpClientFactory httpClientFactory,
        ILogger<EventProcessingBackgroundService> logger)
    {
        _channel = channel;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
                {
                    var client = _httpClientFactory.CreateClient("ExternalMock");
                    var response = await client.PostAsJsonAsync("/api/external-mock", @event, stoppingToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("External mock returned non-success status {StatusCode} for event {@Event}", response.StatusCode, @event);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Event processing canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing event {Event}", @event);
                    // no rethrow: se contin�a procesando otros eventos
                }
        }
    }
}
