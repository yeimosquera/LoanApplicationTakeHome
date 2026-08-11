using System;
using LoanApplication.Api.Domain.Customers;

namespace LoanApplication.Api.Domain.Loans;

public sealed record Application
(
    Guid Id,
    decimal RequestedAmount,
    Guid CustomerId
)
{
    // Navegación (init para que EF Core pueda materializarla)
    public Customer? Customer { get; init; }
}