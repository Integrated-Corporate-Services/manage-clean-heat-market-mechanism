using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class EditAdminUserCommandValidator : AbstractValidator<EditAdminUserCommand>
    {
        public EditAdminUserCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.RoleIds).NotEmpty();
        }
    }
}
