using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LoanApplication.Api.Infrastructure.Messaging;

public sealed class ChannelEventPublisher : IBackgroundEventPublisher
{
    private readonly Channel<ApplicationSavedEvent> _channel;

    public ChannelEventPublisher(Channel<ApplicationSavedEvent> channel)
    {
        _channel = channel;
    }

    public async Task PublishApplicationSavedAsync(ApplicationSavedEvent @event, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }
}