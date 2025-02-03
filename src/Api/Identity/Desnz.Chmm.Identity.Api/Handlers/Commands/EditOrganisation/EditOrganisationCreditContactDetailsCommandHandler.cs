using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationCreditContactDetailsCommandHandler : BaseRequestHandler<EditOrganisationCreditContactDetailsCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationCreditContactDetailsCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationCreditContactDetailsCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationCreditContactDetailsCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, false, false, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            organisation.UpdateCreditContactDetails(request.Name, request.Email, request.TelephoneNumber);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
