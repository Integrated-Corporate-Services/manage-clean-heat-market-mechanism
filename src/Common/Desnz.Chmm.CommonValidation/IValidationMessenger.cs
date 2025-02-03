using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CommonValidation
{
    public interface IValidationMessenger
    {
        BadRequestObjectResult CannotLoadSchemeYear(Guid schemeYearId, Common.ValueObjects.ProblemDetails? problemDetails);
        BadRequestObjectResult CannotLoadOrganisation(Guid organisationId, Common.ValueObjects.ProblemDetails? problemDetails);
        BadRequestObjectResult InvalidOrganisationStatus(Guid organisationId, string status);
        BadRequestObjectResult InvalidLoadSchemeYearQuarter(Guid schemeYearId, Common.ValueObjects.ProblemDetails? problemDetails);
        BadRequestObjectResult InvalidObligationAmendment(Guid organisationId);
    }
}
