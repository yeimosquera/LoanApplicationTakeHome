
using System.Threading;
using System.Threading.Tasks;

namespace LoanApplication.Api.Infrastructure.Messaging;

public interface IBackgroundEventPublisher
{
    Task PublishApplicationSavedAsync(ApplicationSavedEvent @event, CancellationToken cancellationToken = default);
}