using System;

namespace LoanApplication.Api.Domain.Customers;

public sealed record Customer
(
    Guid Id,
    string FirstName,
    string LastName,
    string Address,
    string State,
    string? CompanyName,
    string Ssn
);