using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetOrganisationsQueryHandler : BaseRequestHandler<GetOrganisationsQuery, ActionResult<List<ViewOrganisationDto>>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    public GetOrganisationsQueryHandler(
        ILogger<BaseRequestHandler<GetOrganisationsQuery, ActionResult<List<ViewOrganisationDto>>>> logger,
        IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    public override async Task<ActionResult<List<ViewOrganisationDto>>> Handle(GetOrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = await _organisationsRepository.GetAll(includeLicenceHolderLinks: true);
        return organisations.Select(o =>
            new ViewOrganisationDto
            {
                Id = o.Id,
                Name = o.Name,
                Status = o.Status,
                LicenceHolders = o.GetLinkedLicenceHolders(),
                IsNonSchemeParticipant = o.IsNonSchemeParticipant
            }
        ).ToList();
    }
}
