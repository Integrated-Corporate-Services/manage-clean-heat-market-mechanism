using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class EditManufacturerUserCommandValidator : AbstractValidator<EditManufacturerUserCommand>
    {
        public EditManufacturerUserCommandValidator(ICurrentUserService currentUserService)
        {
            RuleFor(c => c.Id).NotEmpty();

            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters");

            RuleFor(u => u.JobTitle)
                .MaximumLength(100).WithMessage("The JobTitle field has a maximum length of 100 characters")
                .When(x => !string.IsNullOrEmpty(x.JobTitle));

            RuleFor(u => u.TelephoneNumber)
                .NotEmpty().WithMessage("TelephoneNumber is a required field")
                .MaximumLength(100).WithMessage("The TelephoneNumber field has a maximum length of 100 characters")
                .MinimumLength(10).WithMessage("The Telephone Number field has a minimum length of 10 characters")
                .Matches("^[0-9]*$").WithMessage("The TelephoneNumber field should be numeric");

            RuleFor(command => command.OrganisationId)
                .Custom((field, context) =>
                {
                    var currentUser = currentUserService.CurrentUser;

                    if (currentUser.IsAdmin() && (field == Guid.Empty || field == null))
                    {
                        context.AddFailure("A valid OrganisationId value must be provided");
                    }
                });
        }
    }
}
