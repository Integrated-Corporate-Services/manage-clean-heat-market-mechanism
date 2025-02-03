using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators;

public class InviteAdminUserCommandValidator : AbstractValidator<InviteAdminUserCommand>
{
    public InviteAdminUserCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Email).NotEmpty().MaximumLength(100).EmailAddress();
        RuleFor(c => c.RoleIds).NotEmpty();
    }
}
