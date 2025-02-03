using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Extensions;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    public class EditManufacturerUserCommandHandler : BaseRequestHandler<EditManufacturerUserCommand, ActionResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IIdentityNotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOrganisationsRepository _organisationsRepository;

        public EditManufacturerUserCommandHandler(
            ILogger<BaseRequestHandler<EditManufacturerUserCommand, ActionResult>> logger,
            IUsersRepository usersRepository,
            IRolesRepository rolesRepository,
            IIdentityNotificationService notificationService,
            ICurrentUserService currentUserService,
            IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _usersRepository = usersRepository;
            _rolesRepository = rolesRepository;
            _notificationService = notificationService;
            _currentUserService = currentUserService;
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult> Handle(EditManufacturerUserCommand command, CancellationToken cancellationToken)
        {

            var user = await _usersRepository.Get(u => u.Id == command.Id, true, true);
            if (user == null)
                return CannotFindUser(command.Id);

            if (user.OrganisationId == null)
            {
                return NonManufacturerUserEditingAtempted(user.Id);
            }

            var currentUser = _currentUserService.CurrentUser;

            if (!currentUser.IsAdmin() && user.OrganisationId != currentUser.GetOrganisationId())
            {
                return CannotEditUserAcrossOrganisation(user.OrganisationId.Value, user.Id);
            }

            user.UpdateDetails(command.Name, command.JobTitle, command.TelephoneNumber);

            await _usersRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyManufacturerUserEdited(user);

            return Responses.Ok();
        }
    }
}
