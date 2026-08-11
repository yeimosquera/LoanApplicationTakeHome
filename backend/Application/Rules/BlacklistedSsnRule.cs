using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class BlacklistedSsnRule : ILoanRule
{
    private static readonly HashSet<string> Blacklist = new(StringComparer.OrdinalIgnoreCase)
    {
        "000-00-0000" // ejemplo
    };

    public Task<bool> IsSatisfiedAsync(SubmitApplicationCommand command, CancellationToken cancellationToken = default)
    {
        var ok = !Blacklist.Contains(command.Ssn);
        return Task.FromResult(ok);
    }
}