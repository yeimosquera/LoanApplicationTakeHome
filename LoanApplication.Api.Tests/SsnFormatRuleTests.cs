using System;
using LoanApplication.Api.Application.Features.Loans;
using LoanApplication.Api.Application.Rules;
using Xunit;

namespace LoanApplication.Api.Tests
{
    public class SsnFormatRuleTests
    {
        private readonly SsnFormatRule _rule = new SsnFormatRule();

        [Fact]
        public void ReturnsTrue_ForValidSsn()
        {
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,1000m,"123-45-6789");
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.True(ok);
            Assert.Null(reason);
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("12-345-6789")]
        [InlineData("")]
        [InlineData(null)]
        public void ReturnsFalse_ForInvalidSsn(string ssn)
        {
            var ssnValue = ssn ?? string.Empty;
            var cmd = new SubmitApplicationCommand("A","B","a@b.com","Addr","CA",null,1000m,ssnValue);
            var ok = _rule.Evaluate(cmd, out var reason);
            Assert.False(ok);
            Assert.Equal("Invalid SSN format. Required format: XXX-XX-XXXX", reason);
        }
    }
}
