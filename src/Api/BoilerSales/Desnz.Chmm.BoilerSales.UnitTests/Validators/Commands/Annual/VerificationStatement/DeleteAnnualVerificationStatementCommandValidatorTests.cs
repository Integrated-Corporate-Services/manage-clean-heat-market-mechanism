using Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using FluentValidation.TestHelper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Validators.Commands.Annual.VerificationStatement;

public class DeleteAnnualVerificationStatementCommandValidatorTests
{
    private readonly DeleteAnnualVerificationStatementCommandValidator _validator;
    private readonly DeleteAnnualVerificationStatementCommand _command;

    public DeleteAnnualVerificationStatementCommandValidatorTests()
    {
        _validator = new DeleteAnnualVerificationStatementCommandValidator();
    }

    [Fact]
    public void ShouldHaveError_When_OrganisationIdIsEmpty()
    {
        var command = new DeleteAnnualVerificationStatementCommand(Guid.Empty, Guid.NewGuid(), "value");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationId);
    }

    [Fact]
    public void ShouldHaveError_When_SchemeYearIdIsEmpty()
    {
        var command = new DeleteAnnualVerificationStatementCommand(Guid.NewGuid(), Guid.Empty, "value");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.SchemeYearId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldHaveError_When_FileNameIsEmpty(string? fileName)
    {
        var command = new DeleteAnnualVerificationStatementCommand(Guid.NewGuid(), Guid.NewGuid(), fileName);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.FileName);
    }
}
