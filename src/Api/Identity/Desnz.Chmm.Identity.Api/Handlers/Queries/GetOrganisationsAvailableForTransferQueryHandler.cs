using AutoMapper;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetOrganisationsAvailableForTransferQueryHandler : BaseRequestHandler<GetOrganisationsAvailableForTransferQuery, ActionResult<List<ViewOrganisationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly ICurrentUserService _userService;

    public GetOrganisationsAvailableForTransferQueryHandler(
        ILogger<BaseRequestHandler<GetOrganisationsAvailableForTransferQuery, ActionResult<List<ViewOrganisationDto>>>> logger,
        IMapper mapper,
        IOrganisationsRepository organisationsRepository,
        ICurrentUserService userService) : base(logger)
    {
        _mapper = mapper;
        _organisationsRepository = organisationsRepository;
        _userService = userService;
    }

    public override async Task<ActionResult<List<ViewOrganisationDto>>> Handle(GetOrganisationsAvailableForTransferQuery request, CancellationToken cancellationToken)
    {
        var organisations = await _organisationsRepository.GetAll(o =>
            o.Id != request.OrganisationId &&
            o.Status == OrganisationConstants.Status.Active &&
            !o.IsNonSchemeParticipant
        );
        return _mapper.Map<List<ViewOrganisationDto>>(organisations);
    }
}
