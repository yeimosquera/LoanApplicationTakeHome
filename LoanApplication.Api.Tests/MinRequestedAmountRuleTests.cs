using System;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using Xunit;

namespace LoanApplication.Api.Tests
{
    public class MinRequestedAmountRuleTests
    {
        private readonly MinRequestedAmountRule _rule = new MinRequestedAmountRule();

        [Fact]
        public void ReturnsFalse_WhenAmountBelowMinimum()
        {
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,999m,"123-45-6789");
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.False(ok);
            Assert.Equal("Requested amount must be at least $1,000", reason);
        }

        [Fact]
        public void ReturnsTrue_WhenAmountIsAtOrAboveMinimum()
        {
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,1000m,"123-45-6789");
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.True(ok);
            Assert.Null(reason);
        }
    }
}
