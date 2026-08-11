using System;
using System.Net.Http;
using System.Text;
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
        var reader = _channel.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var @event))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("ExternalMock");
                    var json = JsonSerializer.Serialize(@event);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/external-mock", content, stoppingToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("ExternalMock returned {StatusCode} for event {Event}", response.StatusCode, @event);
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
                    // no rethrow: se continúa procesando otros eventos
                }
            }
        }
    }
}