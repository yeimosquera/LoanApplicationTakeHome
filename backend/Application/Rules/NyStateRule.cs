using System;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class NyStateRule : ILoanRule
{
    public bool Evaluate(SubmitApplicationCommand command, out string? denialReason)
    {
        if (string.Equals(command.State, "NY", StringComparison.OrdinalIgnoreCase))
        {
            denialReason = "El estado de NY no está permitido.";
            return false;
        }

        denialReason = null;
        return true;
    }
}