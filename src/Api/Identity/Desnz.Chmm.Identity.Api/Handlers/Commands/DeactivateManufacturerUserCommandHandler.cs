using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public class DeactivateManufacturerUserCommandHandler : BaseRequestHandler<DeactivateManufacturerUserCommand, ActionResult>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateManufacturerUserCommandHandler(
        ILogger<BaseRequestHandler<DeactivateManufacturerUserCommand, ActionResult>> logger,
        IUsersRepository usersRepository,
        IOrganisationsRepository organisationsRepository,
        ICurrentUserService currentUserService) : base(logger)
    {
        _usersRepository = usersRepository;
        _organisationsRepository = organisationsRepository;
        _currentUserService = currentUserService;
    }

    public override async Task<ActionResult> Handle(DeactivateManufacturerUserCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.CurrentUser;

        if (currentUser.GetUserId() == command.UserId)
            return CannotDeactivateYourself();

        var existingUserForId = await _usersRepository.GetById(command.UserId, true, true);
        if (existingUserForId == null)
            return CannotFindUser(command.UserId);

        if (existingUserForId.Status == UsersConstants.Status.Inactive)
            return InvalidUserStatus(command.UserId, existingUserForId.Status);

        var organisation = await _organisationsRepository.GetById(command.OrganisationId, false, true, withTracking: true);
        if (organisation == null)
            return CannotFindOrganisation(command.OrganisationId);

        if (existingUserForId.OrganisationId != command.OrganisationId)
            return CannotDeactivateUserAcrossOrganisation(existingUserForId.OrganisationId.Value, command.UserId);

        existingUserForId.Deactivate();

        _usersRepository.UnitOfWork.SaveChangesAsync();

        return Responses.NoContent();
    }
}
