using MediatR;

namespace LoanApplication.Api.Application.Features.Loans;

public sealed record SubmitApplicationCommand(
    string FirstName,
    string LastName,
    string Email,
    string Address,
    string State,
    string? CompanyName,
    decimal RequestedAmount,
    string Ssn
) : IRequest<SubmitApplicationResult>;