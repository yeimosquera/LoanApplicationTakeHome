using System;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class MaxRequestedAmountRule : ILoanRule
{
    private const decimal MaxAmount = 50000m;

    public bool Evaluate(SubmitApplicationCommand command, out string? denialReason)
    {
        if (command.RequestedAmount > MaxAmount)
        {
            denialReason = "Requested amount exceeds the maximum limit of $50,000";
            return false;
        }

        denialReason = null;
        return true;
    }
}