using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Identity.Api.Entities;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public class InviteManufacturerUserCommandHandler : BaseRequestHandler<InviteManufacturerUserCommand, ActionResult<Guid>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICurrentUserService _userService;
    private readonly IRolesRepository _rolesRepository;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly IIdentityNotificationService _notificationService;

    public InviteManufacturerUserCommandHandler(
        ILogger<BaseRequestHandler<InviteManufacturerUserCommand, ActionResult<Guid>>> logger,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IOrganisationsRepository organisationsRepository,
        ICurrentUserService userService,
        IIdentityNotificationService notificationService
        ) : base(logger)
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _organisationsRepository = organisationsRepository;
        _userService = userService;
        _notificationService = notificationService;
    }

    public override async Task<ActionResult<Guid>> Handle(InviteManufacturerUserCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.ToLower();

        var currentUser = _userService.CurrentUser;
        if (currentUser == null)
            return UserNotAuthenticated();

        var currentUserId = currentUser.GetUserId();
        if (!currentUserId.HasValue)
            return UserNotAuthenticated();

        var existingUserForEmail = await _usersRepository.GetByEmail(email);
        if (existingUserForEmail != null)
        {
            var disclose = (currentUser.IsAdmin() || currentUser.GetOrganisationId() == existingUserForEmail.OrganisationId);
            if (disclose) return UserAlreadyExists(email);
            else return UserAlreadyExistsForDifferentOrganisation();
        }

        var manufacturerRole = await _rolesRepository.GetByName(IdentityConstants.Roles.Manufacturer, true);
        if (manufacturerRole == null)
            return CannotFindRole(IdentityConstants.Roles.Manufacturer);

        var organisation = await _organisationsRepository.GetById(command.OrganisationId, false, true, withTracking: true);
        if (organisation == null)
            return CannotLoadOrganisation(command.OrganisationId);

        var invitingUser = await _usersRepository.GetById(currentUserId.Value);
        if (invitingUser == null)
            return CannotLoadUser(currentUserId.Value);

        var user = new ChmmUser(
            new Common.Dtos.ManufacturerUser.CreateManufacturerUserDto()
            {
                Email = email,
                Name = command.Name,
                JobTitle = command.JobTitle,
                TelephoneNumber = command.TelephoneNumber
            },
            new List<ChmmRole> { manufacturerRole },
            isActive: true,
            currentUser.GetUserId().ToString());
        user.AddToOrganisaton(organisation);

        _ = await _usersRepository.Create(user, true);

        await _notificationService.NotifyManufacturerUserInvited(user, invitingUser.Name);

        return Responses.Created(user.Id);
    }
}
