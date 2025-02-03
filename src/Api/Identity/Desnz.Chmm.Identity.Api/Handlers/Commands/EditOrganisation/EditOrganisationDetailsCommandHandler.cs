using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationDetailsCommandHandler : BaseRequestHandler<EditOrganisationDetailsCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationDetailsCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationDetailsCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationDetailsCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, false, false, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            organisation.UpdateDetails(request.Name, request.CompaniesHouseNumber);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
