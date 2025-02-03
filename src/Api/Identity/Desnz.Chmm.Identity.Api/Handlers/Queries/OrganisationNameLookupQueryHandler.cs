using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class OrganisationNameLookupQueryHandler : BaseRequestHandler<OrganisationNameLookupQuery, ActionResult<List<OrganisationNameDto>>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    public OrganisationNameLookupQueryHandler(
         ILogger<BaseRequestHandler<OrganisationNameLookupQuery, ActionResult<List<OrganisationNameDto>>>> logger,
         IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    public override async Task<ActionResult<List<OrganisationNameDto>>> Handle(OrganisationNameLookupQuery request, CancellationToken cancellationToken)
    {
        var organisations = await _organisationsRepository.GetAll(o => request.Ids.Contains(o.Id));

        if(request.Ids.Except(organisations.Select(i => i.Id)).Any())
            return CannotFindOrganisations(request.Ids.Except(organisations.Select(i => i.Id)).ToList());

        return organisations.Select(i => new OrganisationNameDto { Id = i.Id, Name = i.Name }).ToList();
    }
}
