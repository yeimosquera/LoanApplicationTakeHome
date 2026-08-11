namespace LoanApplication.Api.Infrastructure.Messaging;

public sealed record ApplicationSavedEvent(Guid CustomerId, Guid ApplicationId, bool IsReturningCustomer);