using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationSeniorResponsibleOfficerCommandHandler : BaseRequestHandler<EditOrganisationSeniorResponsibleOfficerCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationSeniorResponsibleOfficerCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationSeniorResponsibleOfficerCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationSeniorResponsibleOfficerCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;

            if (!request.IsApplicantSeniorResponsibleOfficer &&
                (
                    string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.JobTitle) ||
                    string.IsNullOrEmpty(request.TelephoneNumber)
                ))
                return MustSpeficySroDetails(organisationId);

            if (request.IsApplicantSeniorResponsibleOfficer &&
                (
                    !string.IsNullOrEmpty(request.Name) ||
                    !string.IsNullOrEmpty(request.JobTitle) ||
                    !string.IsNullOrEmpty(request.TelephoneNumber)
                ))
                return MustNotSpeficySroDetails(organisationId);

            var organisation = await _organisationsRepository.GetByIdForUpdate(organisationId, false, true, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationId);

            if (request.IsApplicantSeniorResponsibleOfficer)
                organisation.RemoveResponsibleOfficerIfExists();
            else 
                organisation.UpdateSeniorResponsibleOfficerIfExists(request.Name, request.JobTitle, request.TelephoneNumber);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();
            return Responses.NoContent();
        }
    }
}
