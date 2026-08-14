using FluentValidation;
using LoanApplication.Api.Application.Features.Loans;

namespace LoanApplication.Api.Application.Rules;

public sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Length(2,50);
        RuleFor(x => x.LastName).NotEmpty().Length(2,50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Ssn).NotEmpty().Matches("^\\d{3}-\\d{2}-\\d{4}$");
        RuleFor(x => x.State).NotEmpty().Length(2);
        RuleFor(x => x.RequestedAmount).InclusiveBetween(1000m, 50000m);
    }
}