using AutoMapper;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetAdminUserQueryHandler : BaseRequestHandler<GetAdminUserQuery, ActionResult<ChmmUserDto>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public GetAdminUserQueryHandler(
        ILogger<BaseRequestHandler<GetAdminUserQuery, ActionResult<ChmmUserDto>>> logger, 
        IUsersRepository usersRepository, 
        IMapper mapper) : base(logger)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public override async Task<ActionResult<ChmmUserDto>> Handle(GetAdminUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.Get(u => u.Id == request.UserId, true);

        if (user == null)
            return CannotFindUser(request.UserId);

        return _mapper.Map<ChmmUserDto>(user);
    }
}
