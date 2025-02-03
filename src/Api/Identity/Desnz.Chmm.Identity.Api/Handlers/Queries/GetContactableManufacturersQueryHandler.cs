using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetContactableManufacturersQueryHandler : BaseRequestHandler<GetContactableManufacturersQuery, ActionResult<List<OrganisationContactDetailsDto>>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    public GetContactableManufacturersQueryHandler(
         ILogger<BaseRequestHandler<GetContactableManufacturersQuery, ActionResult<List<OrganisationContactDetailsDto>>>> logger,
         IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    public override async Task<ActionResult<List<OrganisationContactDetailsDto>>> Handle(GetContactableManufacturersQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.Get(o => o.Id == request.OrganisationId);
        if (organisation == null)
            return CannotFindOrganisation(request.OrganisationId);
        if (organisation.Status != OrganisationConstants.Status.Active)
            return InvalidOrganisationStatus(request.OrganisationId, organisation.Status);

        var organisations = await _organisationsRepository.GetAll(o => 
        o.ContactName != null && 
        o.Id != request.OrganisationId && 
        o.Status == OrganisationConstants.Status.Active);
        
        return organisations.Select(i => new OrganisationContactDetailsDto { 
            Id = i.Id, 
            OrganisationName = i.Name,
            ContactName = i.ContactName,
            ContactEmail = i.ContactEmail,
            ContactTelephone = i.ContactTelephoneNumber
        }).ToList();
    }
}