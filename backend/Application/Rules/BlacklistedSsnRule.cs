using System;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class BlacklistedSsnRule : ILoanRule
{
    private const string Blacklisted = "000-00-0000";

    public bool Evaluate(SubmitApplicationCommand command, out string? denialReason)
    {
        if (string.Equals(command.Ssn, Blacklisted, StringComparison.OrdinalIgnoreCase))
        {
            denialReason = "SSN is blacklisted";
            return false;
        }

        denialReason = null;
        return true;
    }
}