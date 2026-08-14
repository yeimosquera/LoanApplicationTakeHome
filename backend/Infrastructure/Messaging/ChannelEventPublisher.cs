using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LoanApplication.Api.Infrastructure.Messaging;

public sealed class ChannelEventPublisher : IBackgroundEventPublisher
{
    private readonly Channel<ApplicationSavedEvent> _channel;

    public ChannelEventPublisher(Channel<ApplicationSavedEvent> channel)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    // Interface implementation: publish an already constructed event
    public async Task PublishApplicationSavedAsync(ApplicationSavedEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event is null) throw new ArgumentNullException(nameof(@event));
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }

    // Convenience overload: publish by primitive values
    public async Task PublishApplicationSavedAsync(Guid customerId, Guid applicationId, bool isReturningCustomer, CancellationToken cancellationToken = default)
    {
        var evt = new ApplicationSavedEvent(customerId, applicationId, isReturningCustomer);
        await _channel.Writer.WriteAsync(evt, cancellationToken);
    }
}