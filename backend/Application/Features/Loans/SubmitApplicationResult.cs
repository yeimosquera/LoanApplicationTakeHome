namespace LoanApplication.Api.Application.Features.Loans;

public sealed record SubmitApplicationResult(bool IsApproved, string? DenialReason);