using AutoMapper;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetOrganisationQueryHandler : BaseRequestHandler<GetOrganisationQuery, ActionResult<GetEditableOrganisationDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly ICurrentUserService _userService;

    public GetOrganisationQueryHandler(
        ILogger<BaseRequestHandler<GetOrganisationQuery, ActionResult<GetEditableOrganisationDto>>> logger,
        IMapper mapper,
         IOrganisationsRepository organisationsRepository,
         ICurrentUserService userService) : base(logger)
    {
        _mapper = mapper;
        _organisationsRepository = organisationsRepository;
        _userService = userService;
    }

    public override async Task<ActionResult<GetEditableOrganisationDto>> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.Get(o => o.Id == request.OrganisationId);
        if (organisation == null)
            return CannotFindOrganisation(request.OrganisationId);

        var dto = _mapper.Map<GetEditableOrganisationDto>(organisation);
        dto.Users.ForEach(u => u.IsResponsibleOfficer = u.Id == organisation.ResponsibleOfficerId);
        return dto;
    }
}
