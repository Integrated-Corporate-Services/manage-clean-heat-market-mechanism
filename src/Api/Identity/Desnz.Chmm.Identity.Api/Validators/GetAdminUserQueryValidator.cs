using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Queries;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators;

public class GetAdminUserQueryValidator : AbstractValidator<GetAdminUserQuery>
{
    public GetAdminUserQueryValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}
