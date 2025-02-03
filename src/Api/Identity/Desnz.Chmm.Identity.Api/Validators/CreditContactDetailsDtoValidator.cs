using Desnz.Chmm.Identity.Common.Dtos;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class CreditContactDetailsDtoValidator : AbstractValidator<CreditContactDetailsDto>
    {
        public CreditContactDetailsDtoValidator()
        {
            RuleFor(c => c.Name).MaximumLength(100);
            RuleFor(c => c.Email).NotEmpty().WithMessage(c => $"Either {nameof(c.Email)} or {nameof(c.TelephoneNumber)} must not be empty.").When(c => !string.IsNullOrWhiteSpace(c.Name) && string.IsNullOrWhiteSpace(c.TelephoneNumber)).EmailAddress().MaximumLength(100);
            RuleFor(c => c.TelephoneNumber).NotEmpty().WithMessage(c => $"Either {nameof(c.Email)} or {nameof(c.TelephoneNumber)} must not be empty.").When(c => !string.IsNullOrWhiteSpace(c.Name) && string.IsNullOrWhiteSpace(c.Email)).Matches("^[0-9]*$").MaximumLength(100);
        }
    }
}
