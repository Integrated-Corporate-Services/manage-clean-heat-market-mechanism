using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationRegisteredOfficeAddressCommandHandler : BaseRequestHandler<EditOrganisationRegisteredOfficeAddressCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationRegisteredOfficeAddressCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationRegisteredOfficeAddressCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationRegisteredOfficeAddressCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, true, false, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            organisation.UpdateRegisteredOfficeAddress(
                request.LineOne, request.LineTwo, request.City, request.County, request.Postcode);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
