using Desnz.Chmm.Common.Validators;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Validators.Commands;

public class InstallationPeriodValidatorTests
{
    private readonly InstallationPeriodValidator<ActionResult> _validator;

    public InstallationPeriodValidatorTests()
    {
        _validator = new InstallationPeriodValidator<ActionResult>();
    }

    [Fact]
    public void ShouldHaveError_When_StartDate_IsEmpty()
    {
        var command = new InstallationRequestCommand(DateTime.MinValue, DateTime.MinValue);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.StartDate);
    }

    [Fact]
    public void ShouldHaveError_When_EndDate_IsEmpty()
    {
        var command = new InstallationRequestCommand(new DateTime(2024, 1, 1), DateTime.MinValue);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.EndDate);
    }

    [Fact]
    public void ShouldHaveError_When_EndDate_IsEarlierThan_StartDate()
    {
        var command = new InstallationRequestCommand(new DateTime(2024, 1, 2), new DateTime(2024, 1, 1));

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.EndDate);
    }
}
