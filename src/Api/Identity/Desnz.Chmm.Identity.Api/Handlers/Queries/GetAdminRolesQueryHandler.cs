using AutoMapper;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class GetAdminRolesQueryHandler : BaseRequestHandler<GetAdminRolesQuery, ActionResult<List<RoleDto>>>
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IMapper _mapper;

        public GetAdminRolesQueryHandler(
            ILogger<BaseRequestHandler<GetAdminRolesQuery, ActionResult<List<RoleDto>>>> logger,
            IRolesRepository rolesRepository,
            IMapper mapper) : base(logger)
        {
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<List<RoleDto>>> Handle(GetAdminRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _rolesRepository.GetAdminRoles();
            return _mapper.Map<List<RoleDto>>(roles);
        }
    }
}
