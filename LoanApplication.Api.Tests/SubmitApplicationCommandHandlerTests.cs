using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using LoanApplication.Api.Domain.Customers;
using LoanApplicationEntity = LoanApplication.Api.Domain.Loans.LoanApplication;
using LoanApplication.Api.Infrastructure.Messaging;
using LoanApplication.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace LoanApplication.Api.Tests
{
    public class SubmitApplicationCommandHandlerTests
    {
        private LoanDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<LoanDbContext>()
                            .UseInMemoryDatabase(dbName)
                            .Options;
            return new LoanDbContext(options);
        }

        private class DummyPublisher : IBackgroundEventPublisher
        {
            public List<ApplicationSavedEvent> Published { get; } = new List<ApplicationSavedEvent>();
            public Task PublishApplicationSavedAsync(ApplicationSavedEvent @event, CancellationToken cancellationToken = default)
            {
                Published.Add(@event);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CreateNewApplication_HappyPath_Approved()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var rules = new ILoanRule[] { new NyStateRule(), new BlacklistedSsnRule(), new MinRequestedAmountRule(), new MaxRequestedAmountRule(), new SsnFormatRule() };
            var publisher = new DummyPublisher();
            var handler = new SubmitApplicationCommandHandler(context, rules, publisher);

            var cmd = new SubmitApplicationCommand("John","Doe","john@example.com","123 Main","CA",null,1500m,"123-45-6789");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsApproved);
            Assert.Null(result.DenialReason);

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Ssn == "123-45-6789");
            Assert.NotNull(customer);
            Assert.NotNull(customer.Application);
            Assert.Single(publisher.Published);
        }

        [Fact]
        public async Task UpdateExistingCustomer_RecurrentApplication_UpdatesApplication()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var ctx = CreateContext(dbName))
            {
                var customerId = Guid.NewGuid();
                var appId = Guid.NewGuid();
                var customer = new Customer(customerId, "Old","Name","OldAddr","CA","OldCo","123-45-6789") { Application = new LoanApplicationEntity { Id = appId, RequestedAmount = 1200m, CustomerId = customerId } };
                await ctx.Customers.AddAsync(customer);
                await ctx.SaveChangesAsync();
            }

            using (var ctx = CreateContext(dbName))
            {
                var rules = new ILoanRule[] { new NyStateRule(), new BlacklistedSsnRule(), new MinRequestedAmountRule(), new MaxRequestedAmountRule(), new SsnFormatRule() };
                var publisher = new DummyPublisher();
                var handler = new SubmitApplicationCommandHandler(ctx, rules, publisher);

                var cmd = new SubmitApplicationCommand("John","Updated","john@example.com","NewAddr","CA",null,2000m,"123-45-6789");
                var result = await handler.Handle(cmd, CancellationToken.None);

                Assert.True(result.IsApproved);
                var customer = await ctx.Customers.Include(c => c.Application).FirstOrDefaultAsync(c => c.Ssn == "123-45-6789");
                Assert.NotNull(customer);
                Assert.Equal(2000m, customer.Application.RequestedAmount);
            }
        }

        [Fact]
        public async Task Denied_WhenRuleFails_NYStateRule()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var rules = new ILoanRule[] { new NyStateRule(), new BlacklistedSsnRule(), new MinRequestedAmountRule(), new MaxRequestedAmountRule(), new SsnFormatRule() };
            var publisher = new DummyPublisher();
            var handler = new SubmitApplicationCommandHandler(context, rules, publisher);

            var cmd = new SubmitApplicationCommand("Jane","Doe","jane@example.com","456 Side","NY",null,2000m,"123-45-6789");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.False(result.IsApproved);
            Assert.Equal("El estado de NY no está permitido.", result.DenialReason);
        }
    }
}
