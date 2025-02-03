using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationApplicantCommandHandler : BaseRequestHandler<EditOrganisationApplicantCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationApplicantCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationApplicantCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationApplicantCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, false, true, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            organisation.UpdateApplicant(request.Name, request.JobTitle, request.TelephoneNumber);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
