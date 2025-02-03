using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands.EditOrganisation
{
    public class EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler : BaseRequestHandler<EditOrganisationSeniorResponsibleOfficerAssignedCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler(
            ILogger<BaseRequestHandler<EditOrganisationSeniorResponsibleOfficerAssignedCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditOrganisationSeniorResponsibleOfficerAssignedCommand request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;
            var userid = request.UserId;
            
            var organisation = await _organisationsRepository.GetById(organisationId, false, true, false, true);

            if (organisation == null) 
                return Responses.NotFound();
            
            if (!organisation.ChmmUsers.Any(u => u.Id == userid))
                return CannotAssignSroAsUserIsNotPartOfOrganisation(organisationId, userid);
            
            var user = organisation.ChmmUsers.FirstOrDefault(u => u.Id == userid);

            if(user.Status != "Active")
                return CannotAssignSroAsUserAsTheyAreNotActive(organisationId, userid);
            
            organisation.UpdateSeniorResponsibleOfficerAssigned(userid);
            await _organisationsRepository.UnitOfWork.SaveChangesAsync();

            return Responses.Ok();
        }
    }
}
