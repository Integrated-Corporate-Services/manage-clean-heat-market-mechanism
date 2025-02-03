
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetContactableManufacturersCountQueryHandler : BaseRequestHandler<GetContactableManufacturersCountQuery, ActionResult<int>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    public GetContactableManufacturersCountQueryHandler(
         ILogger<BaseRequestHandler<GetContactableManufacturersCountQuery, ActionResult<int>>> logger,
         IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    public override async Task<ActionResult<int>> Handle(GetContactableManufacturersCountQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.Get(o => o.Id == request.OrganisationId);
        if (organisation == null)
            return CannotFindOrganisation(request.OrganisationId);
        if (organisation.Status != OrganisationConstants.Status.Active)
            return InvalidOrganisationStatus(request.OrganisationId, organisation.Status);

        var count = await _organisationsRepository.Count(o =>
            o.ContactName != null && 
            o.Id != request.OrganisationId && 
            o.Status == OrganisationConstants.Status.Active);

        return count;
    }
}