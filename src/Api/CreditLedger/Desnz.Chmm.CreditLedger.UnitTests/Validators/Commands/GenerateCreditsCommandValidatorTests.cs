using FluentValidation.TestHelper;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Validators.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.CreditLedger.UnitTests.Validators.Commands
{
    public class GenerateCreditsCommandValidatorTests
    {
        private readonly GenerateCreditsCommandValidator _validator;

        public GenerateCreditsCommandValidatorTests()
        {
            
            _validator = new GenerateCreditsCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Installations_IsEmpty()
        {
            var command = new GenerateCreditsCommand(new List<McsInstallationDto>());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Installations);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Installations_IsNotEmpty()
        {
            var command = new GenerateCreditsCommand(new List<McsInstallationDto> { new McsInstallationDto()});
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.Installations);
        }
     }
}