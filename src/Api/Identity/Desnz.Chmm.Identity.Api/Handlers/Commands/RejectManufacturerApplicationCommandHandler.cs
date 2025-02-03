using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Handlers;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationFileConstants;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    public class RejectManufacturerApplicationCommandHandler : BaseRequestHandler<RejectManufacturerApplicationCommand, ActionResult>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly IOrganisationDecisionCommentsRepository _organisationCommentsRepository;
        private readonly IOrganisationDecisionFilesRepository _organisationDecisionFilesRepository;
        private readonly IFileService _fileService;
        private readonly IUsersRepository _usersRepository;
        private readonly IIdentityNotificationService _notificationService;
        private readonly ICurrentUserService _userService;

        public RejectManufacturerApplicationCommandHandler(
            ILogger<BaseRequestHandler<RejectManufacturerApplicationCommand, ActionResult>> logger,
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
        public override async Task<ActionResult> Handle(RejectManufacturerApplicationCommand command, CancellationToken cancellationToken)
        {
            var organisation = await _organisationsRepository.Get(o => o.Id == command.OrganisationId, withTracking: true);
            if (organisation == null)
                return CannotFindOrganisation(command.OrganisationId.Value);

            organisation.Archive();

            await DeactivateOrganisationUsers(organisation);

            var saveDecisionFilesError = await SaveDecisionFiles(command);
            if (saveDecisionFilesError != null) return saveDecisionFilesError;

            await SaveRejectionComment(command);

            await _organisationsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyManufacturerRejected(organisation);
            return Responses.NoContent();
        }

        private async Task DeactivateOrganisationUsers(Organisation organisation)
        {
            var responsibleOfficer = await _usersRepository.Get(u => u.Id == organisation.ResponsibleOfficerId, withTracking: true);
            responsibleOfficer?.Deactivate();

            if (organisation.ApplicantId == organisation.ResponsibleOfficerId) return;

            var applicant = await _usersRepository.Get(u => u.Id == organisation.ApplicantId, withTracking: true);
            applicant?.Deactivate();
        }

        private async Task<BadRequestObjectResult?> SaveDecisionFiles(RejectManufacturerApplicationCommand command)
        {
            var fileNames = await _fileService.GetFileNamesAsync(Buckets.IdentityOrganisationRejections, $"{command.OrganisationId}");

            foreach (var fileName in fileNames)
            {
                await _organisationDecisionFilesRepository.Create(new OrganisationDecisionFile(
                    fileName, $"{command.OrganisationId}/{fileName}", command.OrganisationId.Value, Decisions.Reject), false);
            }

            return null;
        }

        private async Task SaveRejectionComment(RejectManufacturerApplicationCommand command)
        {
            var adminId = _userService.CurrentUser.GetUserId().Value;
            var organisationComment = new OrganisationDecisionComment(command.Comment ?? string.Empty, adminId, command.OrganisationId.Value, Decisions.Reject);
            await _organisationCommentsRepository.Create(organisationComment, false);
        }
    }
}