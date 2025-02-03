using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetActiveOrganisationsQueryHandler : BaseRequestHandler<GetActiveOrganisationsQuery, ActionResult<List<ViewOrganisationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrganisationsRepository _organisationsRepository;

    public GetActiveOrganisationsQueryHandler(
        ILogger<BaseRequestHandler<GetActiveOrganisationsQuery, ActionResult<List<ViewOrganisationDto>>>> logger,
        IMapper mapper,
         IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _mapper = mapper;
        _organisationsRepository = organisationsRepository;
    }

    public override async Task<ActionResult<List<ViewOrganisationDto>>> Handle(GetActiveOrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = await _organisationsRepository.GetAllActive();
        return _mapper.Map<List<ViewOrganisationDto>>(organisations);
    }
}
