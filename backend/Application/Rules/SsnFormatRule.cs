using System;
using System.Text.RegularExpressions;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class SsnFormatRule : ILoanRule
{
    private static readonly Regex SsnRegex = new Regex("^\\d{3}-\\d{2}-\\d{4}$", RegexOptions.Compiled);

    public bool Evaluate(SubmitApplicationCommand command, out string? denialReason)
    {
        if (string.IsNullOrWhiteSpace(command.Ssn) || !SsnRegex.IsMatch(command.Ssn))
        {
            denialReason = "Invalid SSN format. Required format: XXX-XX-XXXX";
            return false;
        }

        denialReason = null;
        return true;
    }
}