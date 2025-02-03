using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationFossilFuelSellerCommandHandler : BaseRequestHandler<EditOrganisationFossilFuelSellerCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationFossilFuelSellerCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationFossilFuelSellerCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationFossilFuelSellerCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, false, false, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            organisation.UpdateFossilFuelSeller(request.IsFossilFuelBoilerSeller);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
