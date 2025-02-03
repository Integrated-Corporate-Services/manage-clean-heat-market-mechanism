using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Validators.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;

namespace Desnz.Chmm.CreditLedger.UnitTests.Validators.Commands
{
    public class TransferCreditsCommandValidatorTests
    {
        private readonly TransferCreditsCommandValidator _validator;

        public TransferCreditsCommandValidatorTests()
        {
            _validator = new TransferCreditsCommandValidator();
        }

        [Fact]
        public void Value_Too_Small()
        {
            var query = new TransferCreditsCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0);
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Value);
        }

        [Fact]
        public void Value_Negative()
        {
            var query = new TransferCreditsCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -10);
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Value);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0.5)]
        public void Value_Allow_WholeOrExactHalf(decimal inValue)
        {
            var query = new TransferCreditsCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), inValue);
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.Value);
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(0.0001)]
        [InlineData(999.99999)]
        public void Value_DoesNotAllow_Non_WholeOrExactHalf(decimal inValue)
        {
            var query = new TransferCreditsCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), inValue);
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Value);
        }
    }
}