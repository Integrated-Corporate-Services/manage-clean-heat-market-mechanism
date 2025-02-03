using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.CommonValidation
{
    public class RequestValidator : IRequestValidator
    {
        private readonly ICurrentUserService _userService;
        private readonly IOrganisationService _organisationService;
        private readonly ISchemeYearService _schemeYearService;
        private readonly IValidationMessenger _validationMessenger;

        public RequestValidator(
            ICurrentUserService userService,
            IOrganisationService organisationService,
            ISchemeYearService schemeYearService,
            IValidationMessenger validationMessenger)
        {
            _userService = userService;
            _organisationService = organisationService;
            _schemeYearService = schemeYearService;
            _validationMessenger = validationMessenger;
        }

        private async Task<ActionResult?> ValidateOrganisationExists(
            OrganisationValidation? organisationValidation = null)
        {
            if (organisationValidation != null)
            {
                var orgResponse = await _organisationService.GetStatus(organisationValidation.OrganisationId);
                if (!orgResponse.IsSuccessStatusCode || orgResponse.Result == null)
                    return _validationMessenger.CannotLoadOrganisation(organisationValidation.OrganisationId, orgResponse.Problem);

                if (organisationValidation.RequireActiveOrganisation)
                {
                    if (orgResponse.Result.Status != OrganisationConstants.Status.Active)
                        return _validationMessenger.InvalidOrganisationStatus(organisationValidation.OrganisationId, orgResponse.Result.Status);
                }
            }

            return null;
        }

        private async Task<ActionResult?> ValidateSchemeYearExists(
            Guid? schemeYearId = null,
            Guid? schemeYearQuarterId = null,
            CustomValidator<SchemeYearDto>? schemeYearValidation = null)
        {
            // Let's just check that people are passing in the right parameters first.
            if (schemeYearId == null && schemeYearQuarterId != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearQuarterId to be validated");
            if (schemeYearId == null && schemeYearValidation != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearValidation to be validated");

            if (schemeYearId != null)
            {
                var yearResponse = await _schemeYearService.GetSchemeYear(schemeYearId.Value, CancellationToken.None);
                if (!yearResponse.IsSuccessStatusCode || yearResponse.Result == null)
                    return _validationMessenger.CannotLoadSchemeYear(schemeYearId.Value, yearResponse.Problem);

                var schemeYear = yearResponse.Result;

                if (schemeYearQuarterId != null)
                {
                    if (schemeYear.Quarters == null || !schemeYear.Quarters.Select(x => x.Id).Contains(schemeYearQuarterId))
                        return _validationMessenger.InvalidLoadSchemeYearQuarter(schemeYearId.Value, yearResponse.Problem);
                }

                if (schemeYearValidation != null)
                {
                    if (schemeYearValidation.ValidationFunction.Invoke(schemeYear))
                    {
                        return schemeYearValidation.FailureAction(schemeYear);
                    }
                }
            }

            return null;
        }

        public async Task<ActionResult?> ValidateSchemeYearAndOrganisation(
            Guid? organisationId = null,
            bool requireActiveOrganisation = false,
            Guid? schemeYearId = null,
            Guid? schemeYearQuarterId = null,
            CustomValidator<SchemeYearDto>? schemeYearValidation = null)
        {
            // Let's just check that people are passing in the right parameters first.
            if (schemeYearId == null && schemeYearQuarterId != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearQuarterId to be validated");
            if (schemeYearId == null && schemeYearValidation != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearValidation to be validated");

            // Let's just check that people are passing in the right parameters first.
            if (organisationId == null && requireActiveOrganisation == true)
                throw new ArgumentNullException(nameof(organisationId), "organisationId cannot be null if you want requireActiveOrganisation to be validated");

            if (organisationId != null)
            {
                var orgValidation = await ValidateOrganisationExists(
                    new OrganisationValidation
                    {
                        OrganisationId = organisationId.Value,
                        RequireActiveOrganisation = requireActiveOrganisation,
                    });

                if (orgValidation != null)
                    return orgValidation;
            }

            var yearValidation = await ValidateSchemeYearExists(schemeYearId, schemeYearQuarterId, schemeYearValidation);

            if (yearValidation != null)
                return yearValidation;

            return null;
        }

        public async Task<ActionResult?> ValidateSchemeYearAndOrganisation(
            List<OrganisationValidation> organisationValidateions,
            Guid? schemeYearId = null,
            Guid? schemeYearQuarterId = null,
            CustomValidator<SchemeYearDto>? schemeYearValidation = null)
        {
            // Let's just check that people are passing in the right parameters first.
            if (schemeYearId == null && schemeYearQuarterId != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearQuarterId to be validated");
            if (schemeYearId == null && schemeYearValidation != null)
                throw new ArgumentNullException(nameof(schemeYearId), "schemeYearId cannot be null if you want schemeYearValidation to be validated");

            foreach (var organisationValidateion in organisationValidateions)
            {
                var orgValidation = await ValidateOrganisationExists(organisationValidateion);

                if (orgValidation != null)
                    return orgValidation;
            }

            var yearValidation = await ValidateSchemeYearExists(schemeYearId, schemeYearQuarterId, schemeYearValidation);

            if (yearValidation != null)
                return yearValidation;

            return null;
        }
    }
}
