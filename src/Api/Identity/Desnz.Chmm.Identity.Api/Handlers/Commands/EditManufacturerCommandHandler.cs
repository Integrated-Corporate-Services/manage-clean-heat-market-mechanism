using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    public class EditManufacturerCommandHandler : BaseRequestHandler<EditManufacturerCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly IOrganisationDecisionCommentsRepository _organisationCommentsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IIdentityNotificationService _notificationService;
        private readonly ICurrentUserService _userService;

        public EditManufacturerCommandHandler(
            ILogger<BaseRequestHandler<EditManufacturerCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository,
            IOrganisationDecisionCommentsRepository organisationCommentsRepository,
            IUsersRepository usersRepository,
            IIdentityNotificationService notificationService,
            ICurrentUserService userService) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
            _organisationCommentsRepository = organisationCommentsRepository;
            _usersRepository = usersRepository;
            _notificationService = notificationService;
            _userService = userService;
        }

        public override async Task<ActionResult> Handle(EditManufacturerCommand command, CancellationToken cancellationToken)
        {
            var organisationDetails = JsonConvert.DeserializeObject<EditOrganisationDto>(command.OrganisationDetailsJson)!;
            
            var organisation = await _organisationsRepository.Get(o => o.Id == organisationDetails.Id, null, true);
            if (organisation == null)
                return CannotFindOrganisation(organisationDetails.Id);

            var updatedLegalAddress = organisationDetails.Addresses.SingleOrDefault(x => x.IsUsedAsLegalCorrespondence);
            var legalAddress = organisation.Addresses.SingleOrDefault(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);

            if (updatedLegalAddress != null && legalAddress == null)
            {
                await _organisationsRepository.CreateAddress(organisation.Id, new OrganisationAddress(updatedLegalAddress));
            }

            organisation.UpdateOrganisationDetails(organisationDetails);

            if (command.Comment != null)
            {
                var adminEmail = _userService.CurrentUser.GetEmail();
                var user = await _usersRepository.Get(x => x.Email == adminEmail);
                var comment = new OrganisationDecisionComment(command.Comment, user.Id, organisation.Id, OrganisationFileConstants.Decisions.Approve);
                await _organisationCommentsRepository.Create(comment, false);
            }

            await _organisationsRepository.UnitOfWork.SaveChangesAsync();

            await _notificationService.NotifyOrganisationEdited(organisation);

            return Responses.NoContent();
        }
    }
}
