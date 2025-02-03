using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class InviteManufacturerUserCommandValidator : AbstractValidator<InviteManufacturerUserCommand>
    {
        public InviteManufacturerUserCommandValidator(int? count = null)
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is a required field")
                .EmailAddress()
                .MaximumLength(100).WithMessage("The Email field has a maximum length of 100 characters");

            RuleFor(u => u.JobTitle)
                .NotEmpty().WithMessage("The Job Title is a required field")
                .MaximumLength(100).WithMessage("The Job Title field has a maximum length of 100 characters");

            RuleFor(u => u.TelephoneNumber)
                .NotEmpty().WithMessage("TelephoneNumber is a required field")
                .MinimumLength(10).WithMessage("The Telephone Number field has a minimum length of 10 characters")
                .MaximumLength(100).WithMessage("The Telephone Number field has a maximum length of 100 characters")
                .Matches("^[0-9]*$").WithMessage("The Telephone Number field should be numeric");
        }
    }
}
