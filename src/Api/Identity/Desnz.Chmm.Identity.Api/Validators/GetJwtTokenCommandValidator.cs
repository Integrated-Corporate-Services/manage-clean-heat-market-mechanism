using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators;

public class GetJwtTokenCommandValidator : AbstractValidator<GetJwtTokenCommand>
{
    public GetJwtTokenCommandValidator()
    {
        RuleFor(c => c.IdToken).NotEmpty().When(c => string.IsNullOrEmpty(c.Email) && string.IsNullOrEmpty(c.ApiKey));
        RuleFor(c => c.AccessToken).NotEmpty().When(c => string.IsNullOrEmpty(c.Email) && string.IsNullOrEmpty(c.ApiKey));
        RuleFor(c => c.Email).NotEmpty().When(c => string.IsNullOrEmpty(c.IdToken) && string.IsNullOrEmpty(c.AccessToken) && string.IsNullOrEmpty(c.ApiKey));
        RuleFor(c => c.ApiKey).NotEmpty().When(c => string.IsNullOrEmpty(c.IdToken) && string.IsNullOrEmpty(c.AccessToken) && string.IsNullOrEmpty(c.Email));
    }
}
