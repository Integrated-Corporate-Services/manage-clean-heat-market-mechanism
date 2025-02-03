using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Handlers;
using static Desnz.Chmm.Identity.Common.Constants.OrganisationConstants;
using Desnz.Chmm.Identity.Api.Constants;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    public class ApproveManufacturerApplicationCommandHandler : BaseRequestHandler<ApproveManufacturerApplicationCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly IOrganisationDecisionCommentsRepository _organisationCommentsRepository;
        private readonly IOrganisationDecisionFilesRepository _organisationDecisionFilesRepository;
        private readonly IFileService _fileService;
        private readonly IUsersRepository _usersRepository;
        private readonly IIdentityNotificationService _notificationService;
        private readonly ICurrentUserService _userService;

        public ApproveManufacturerApplicationCommandHandler(
            ILogger<BaseRequestHandler<ApproveManufacturerApplicationCommand, ActionResult>> logger,
            IOrganisationsRepository organisationsRepository,
            IOrganisationDecisionCommentsRepository organisationCommentsRepository,
            IOrganisationDecisionFilesRepository organisationDecisionFilesRepository,
            IUsersRepository usersRepository,
            IFileService fileService,
            IIdentityNotificationService notificationService,
            ICurrentUserService userService) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
            _organisationCommentsRepository = organisationCommentsRepository;
            _organisationDecisionFilesRepository = organisationDecisionFilesRepository;
            _fileService = fileService;
            _usersRepository = usersRepository;
            _notificationService = notificationService;
            _userService = userService;
        }
        public override async Task<ActionResult> Handle(ApproveManufacturerApplicationCommand command, CancellationToken cancellationToken)
        {
            var organisation = await _organisationsRepository.Get(o => o.Id == command.OrganisationId, withTracking: true);
            if (organisation == null)
                return CannotFindOrganisation(command.OrganisationId.Value);

            organisation.Activate();

            await ActivateOrganisationUsers(organisation);

            var saveDecisionFilesError = await SaveDecisionFiles(command);
            if (saveDecisionFilesError != null) return saveDecisionFilesError;

            await SaveDecisionComment(command);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyManufacturerApproved(organisation);
            return Responses.NoContent();
        }

        private async Task ActivateOrganisationUsers(Organisation organisation)
        {
            var responsibleOfficer = await _usersRepository.Get(u => u.Id == organisation.ResponsibleOfficerId, withTracking: true);
            responsibleOfficer?.Activate();

            if (organisation.ApplicantId == organisation.ResponsibleOfficerId) return;

            var applicant = await _usersRepository.Get(u => u.Id == organisation.ApplicantId, withTracking: true);
            applicant?.Activate();
        }

        private async Task<BadRequestObjectResult?> SaveDecisionFiles(ApproveManufacturerApplicationCommand command)
        {
            var fileNames = await _fileService.GetFileNamesAsync(Buckets.IdentityOrganisationApprovals, $"{command.OrganisationId}");

            foreach (var fileName in fileNames)
            {
                await _organisationDecisionFilesRepository.Create(new OrganisationDecisionFile(
                    fileName, $"{command.OrganisationId}/{fileName}", command.OrganisationId.Value, OrganisationFileConstants.Decisions.Approve), false);
            }

            return null;
        }

        private async Task SaveDecisionComment(ApproveManufacturerApplicationCommand command)
        {
            var adminId = _userService.CurrentUser.GetUserId().Value;
            var organisationComment = new OrganisationDecisionComment(command.Comment ?? string.Empty, adminId, command.OrganisationId.Value, OrganisationFileConstants.Decisions.Approve);
            await _organisationCommentsRepository.Create(organisationComment, false);
        }
    }
}