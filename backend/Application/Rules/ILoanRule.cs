using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public interface ILoanRule
{
    bool Evaluate(SubmitApplicationCommand command, out string? denialReason);
}