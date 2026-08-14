using System;
using LoanApplication.Api.Domain.Customers;

namespace LoanApplication.Api.Domain.Loans;

public sealed class LoanApplication
{
    public Guid Id { get; set; }
    public decimal RequestedAmount { get; set; }

    // FK y navegación
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
}