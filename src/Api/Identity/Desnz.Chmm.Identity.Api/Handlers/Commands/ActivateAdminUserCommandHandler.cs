using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public class ActivateAdminUserCommandHandler : BaseRequestHandler<ActivateAdminUserCommand, ActionResult>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IIdentityNotificationService _notificationService;

    public ActivateAdminUserCommandHandler(
        ILogger<BaseRequestHandler<ActivateAdminUserCommand, ActionResult>> logger,
        IUsersRepository usersRepository,
        IIdentityNotificationService notificationService): base(logger)
    {
        _usersRepository = usersRepository;
        _notificationService = notificationService;
    }

    public override async Task<ActionResult> Handle(ActivateAdminUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.Get(u => u.Id == command.Id, withTracking: true);

        if (user == null)
            return CannotLoadUser(command.Id);

        if (user.Status != UsersConstants.Status.Inactive)
            return InvalidUserStatus(command.Id, user.Status);

        user.Activate();

        await _usersRepository.UnitOfWork.SaveChangesAsync();

        await _notificationService.NotifyUserActivated(user);

        return Responses.NoContent();
    }
}
