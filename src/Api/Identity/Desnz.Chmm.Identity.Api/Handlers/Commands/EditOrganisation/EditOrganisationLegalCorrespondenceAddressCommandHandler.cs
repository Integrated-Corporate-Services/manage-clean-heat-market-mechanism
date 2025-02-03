using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationLegalCorrespondenceAddressCommandHandler : BaseRequestHandler<EditOrganisationLegalCorrespondenceAddressCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationLegalCorrespondenceAddressCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationLegalCorrespondenceAddressCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationLegalCorrespondenceAddressCommand request, CancellationToken cancellationToken)
        { 
            var organisationId = request.OrganisationId;

            if (request.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress &&
                (
                    string.IsNullOrEmpty(request.LineOne) ||
                    string.IsNullOrEmpty(request.City) ||
                    string.IsNullOrEmpty(request.Postcode)
                ))
                return MustSpeficyAddressDetails(organisationId, request.LegalAddressType);

            if (request.LegalAddressType != LegalCorrespondenceAddressType.UseSpecifiedAddress &&
                (
                    !string.IsNullOrEmpty(request.LineOne) ||
                    !string.IsNullOrEmpty(request.LineTwo) ||
                    !string.IsNullOrEmpty(request.City) ||
                    !string.IsNullOrEmpty(request.County) ||
                    !string.IsNullOrEmpty(request.Postcode)
                ))
                return MustNotSpeficyAddressDetails(organisationId, request.LegalAddressType);

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, true, false, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            if (request.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress)
            {
                var address = organisation.UpdateLegalCorrespondenceAddress(request.LegalAddressType, request.LineOne, request.LineTwo, request.City, request.County, request.Postcode);
                if (address == null)
                {
                    await _organisationsRepository.CreateAddress(organisationId, new OrganisationAddress(new CreateOrganisationAddressDto
                    {
                        LineOne = request.LineOne,
                        LineTwo = request.LineTwo,
                        City = request.City,
                        County = request.County,
                        Postcode = request.Postcode,
                        IsUsedAsLegalCorrespondence = true
                    }), false);
                }
            }
            else
                organisation.RemoveLegalCorrespondenceAddressIfExists(request.LegalAddressType);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();

            return Responses.NoContent();
        }
    }
}
