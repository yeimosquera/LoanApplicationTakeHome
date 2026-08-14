using System;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class MinRequestedAmountRule : ILoanRule
{
    private const decimal MinAmount = 1000m;

    public bool Evaluate(SubmitApplicationCommand command, out string? denialReason)
    {
        if (command.RequestedAmount < MinAmount)
        {
            denialReason = "Requested amount must be at least $1,000";
            return false;
        }

        denialReason = null;
        return true;
    }
}