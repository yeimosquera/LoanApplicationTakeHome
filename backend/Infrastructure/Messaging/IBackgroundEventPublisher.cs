using System.Threading;

namespace LoanApplication.Api.Infrastructure.Messaging;

public interface IBackgroundEventPublisher
{
    Task PublishApplicationSavedAsync(ApplicationSavedEvent @event, CancellationToken cancellationToken = default);
}