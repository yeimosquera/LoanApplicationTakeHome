using System.Threading;
using System.Threading.Tasks;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class NyStateRule : ILoanRule
{
    public Task<bool> IsSatisfiedAsync(SubmitApplicationCommand command, CancellationToken cancellationToken = default)
    {
        // Regla de ejemplo mínima: no impide por ahora; placeholder simple.
        return Task.FromResult(true);
    }
}