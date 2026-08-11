using System.Threading;
using System.Threading.Tasks;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public interface ILoanRule
{
    Task<bool> IsSatisfiedAsync(SubmitApplicationCommand command, CancellationToken cancellationToken = default);
}