using AutoMapper;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Queries;
using Desnz.Chmm.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetManufacturerUserQueryHandler : BaseRequestHandler<GetManufacturerUserQuery, ActionResult<ViewManufacturerUserDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public GetManufacturerUserQueryHandler(
        ILogger<BaseRequestHandler<GetManufacturerUserQuery, ActionResult<ViewManufacturerUserDto>>> logger,
        ICurrentUserService currentUserService,
        IOrganisationsRepository organisationsRepository,
        IUsersRepository usersRepository,
        IMapper mapper) : base(logger)
    {
        _currentUserService = currentUserService;
        _organisationsRepository = organisationsRepository;
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public override async Task<ActionResult<ViewManufacturerUserDto>> Handle(GetManufacturerUserQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.Get(o => o.Id == request.OrganisationId);
        if (organisation == null)
            return CannotFindOrganisation(request.OrganisationId);

        var user = await _usersRepository.Get(u => u.Id == request.UserId && u.OrganisationId == request.OrganisationId, true);
        if (user == null)
            return CannotFindUser(request.UserId);

        return _mapper.Map<ViewManufacturerUserDto>(user);
    }
}
