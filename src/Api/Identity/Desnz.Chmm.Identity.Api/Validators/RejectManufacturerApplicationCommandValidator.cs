using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands;

namespace Desnz.Chmm.Identity.Api.Validators;

public class RejectManufacturerApplicationCommandValidator : AbstractValidator<RejectManufacturerApplicationCommand>
{
    public RejectManufacturerApplicationCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.Comment).MaximumLength(255).When(c => !string.IsNullOrWhiteSpace(c.Comment));
    }
}
