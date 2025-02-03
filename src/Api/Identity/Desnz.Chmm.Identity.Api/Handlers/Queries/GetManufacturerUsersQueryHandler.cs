using AutoMapper;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetManufacturerUsersQueryHandler : BaseRequestHandler<GetManufacturerUsersQuery, ActionResult<List<ViewManufacturerUserDto>>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _userService;


    public GetManufacturerUsersQueryHandler(
        ILogger<BaseRequestHandler<GetManufacturerUsersQuery, ActionResult<List<ViewManufacturerUserDto>>>> logger,
        IUsersRepository usersRepository,
        IMapper mapper,
        ICurrentUserService userService) : base(logger)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public override async Task<ActionResult<List<ViewManufacturerUserDto>>> Handle(GetManufacturerUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetAll(u => u.OrganisationId == request.OrganisationId);
        return _mapper.Map<List<ViewManufacturerUserDto>>(users);
    }
}