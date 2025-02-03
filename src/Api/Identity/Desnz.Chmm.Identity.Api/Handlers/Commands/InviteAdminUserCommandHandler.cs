using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public class InviteAdminUserCommandHandler : BaseRequestHandler<InviteAdminUserCommand, ActionResult<Guid>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IIdentityNotificationService _notificationService;

    public InviteAdminUserCommandHandler(
        ILogger<BaseRequestHandler<InviteAdminUserCommand, ActionResult<Guid>>> logger,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IIdentityNotificationService notificationService) : base(logger)
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _notificationService = notificationService;
    }

    public override async Task<ActionResult<Guid>> Handle(InviteAdminUserCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.ToLower();
        var existingUserForEmail = await _usersRepository.GetByEmail(email);
        if (existingUserForEmail != null)
            return UserAlreadyExists(email);

        var roles = await _rolesRepository.GetAll(x => command.RoleIds.Contains(x.Id), withTracking: true);
        if (roles.Count != command.RoleIds.Count)
            return CannotLoadRole(command.RoleIds.Except(roles.Select(x => x.Id)));

        var user = new ChmmUser(command.Name, email, roles);
        var userId = await _usersRepository.Create(user);

        await _notificationService.NotifyAdminUserInvited(user);

        return Responses.Created(userId);
    }
}