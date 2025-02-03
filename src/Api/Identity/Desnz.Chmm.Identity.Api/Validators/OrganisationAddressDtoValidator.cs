using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class OrganisationAddressDtoValidator : AbstractValidator<EditOrganisationAddressDto>
    {
        public OrganisationAddressDtoValidator()
        {
            RuleFor(a => a.IsUsedAsLegalCorrespondence).NotNull();
            RuleFor(a => a.LineOne).NotEmpty().MaximumLength(100);
            RuleFor(a => a.LineTwo).MaximumLength(100).When(a => !string.IsNullOrEmpty(a.LineTwo));
            RuleFor(a => a.City).NotEmpty().MaximumLength(100);
            RuleFor(a => a.County).MaximumLength(100).When(a => !string.IsNullOrEmpty(a.County));
            RuleFor(a => a.Postcode).NotEmpty().MaximumLength(100);
        }
    }
}
