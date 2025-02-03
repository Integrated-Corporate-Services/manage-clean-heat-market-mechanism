using AutoMapper;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetAdminUsersQueryHandler : BaseRequestHandler<GetAdminUsersQuery, ActionResult<List<ChmmUserDto>>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public GetAdminUsersQueryHandler(
        ILogger<BaseRequestHandler<GetAdminUsersQuery, ActionResult<List<ChmmUserDto>>>> logger,
        IUsersRepository usersRepository,
        IMapper mapper): base(logger)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public override async Task<ActionResult<List<ChmmUserDto>>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetAdmins();
        return _mapper.Map<List<ChmmUserDto>>(users);
    }
}

