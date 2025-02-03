using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    public class EditAdminUserCommandHandler : BaseRequestHandler<EditAdminUserCommand, ActionResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IIdentityNotificationService _notificationService;


        public EditAdminUserCommandHandler(
            ILogger<BaseRequestHandler<EditAdminUserCommand, ActionResult>> logger,
            IUsersRepository usersRepository,
            IRolesRepository rolesRepository,
            IIdentityNotificationService notificationService) : base(logger)
        {
            _usersRepository = usersRepository;
            _rolesRepository = rolesRepository;
            _notificationService = notificationService;
        }

        public override async Task<ActionResult> Handle(EditAdminUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.Get(u => u.Id == command.Id, true, true);
            if (user == null)
                return CannotFindUser(command.Id);

            user.SetName(command.Name);

            var roles = await _rolesRepository.GetAll(x => command.RoleIds.Contains(x.Id), true);

            if (roles.Count != command.RoleIds.Count)
                return CannotLoadRole(command.RoleIds.Except(roles.Select(x => x.Id)));

            user.UpdateRoles(roles);

            await _usersRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAdminUserEdited(user);

            return Responses.Ok();
        }
    }
}
