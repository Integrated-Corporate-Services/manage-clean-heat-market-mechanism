using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.CommonValidation
{
    public interface IRequestValidator
    {
        Task<ActionResult?> ValidateSchemeYearAndOrganisation(
            Guid? organisationId = null,
            bool requireActiveOrganisation = false,
            Guid? schemeYearId = null,
            Guid? schemeYearQuarterId = null,
            CustomValidator<SchemeYearDto>? schemeYearValidation = null);

        Task<ActionResult?> ValidateSchemeYearAndOrganisation(
            List<OrganisationValidation> organisationValidations,
            Guid? schemeYearId = null,
            Guid? schemeYearQuarterId = null,
            CustomValidator<SchemeYearDto>? schemeYearValidation = null);
    }
}
