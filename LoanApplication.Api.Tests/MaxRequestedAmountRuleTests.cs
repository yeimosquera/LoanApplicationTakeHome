using System;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using Xunit;

namespace LoanApplication.Api.Tests
{
    public class MaxRequestedAmountRuleTests
    {
        private readonly MaxRequestedAmountRule _rule = new MaxRequestedAmountRule();

        [Fact]
        public void ReturnsFalse_WhenAmountExceedsMax()
        {
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,50001m,"123-45-6789");
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.False(ok);
            Assert.Equal("Requested amount exceeds the maximum limit of $50,000", reason);
        }

        [Fact]
        public void ReturnsTrue_WhenAmountIsAtOrBelowMax()
        {
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,50000m,"123-45-6789");
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.True(ok);
            Assert.Null(reason);
        }
    }
}
