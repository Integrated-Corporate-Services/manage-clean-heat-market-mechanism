using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Handle the get linked licence holders query
/// </summary>
public class GetLicenceHolderLinksQueryHandler : BaseRequestHandler<GetLicenceHolderLinksQuery, ActionResult<List<LicenceHolderLinkDto>>>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOrganisationsRepository _organisationsRepository;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public GetLicenceHolderLinksQueryHandler(
        ILogger<BaseRequestHandler<GetLicenceHolderLinksQuery, ActionResult<List<LicenceHolderLinkDto>>>> logger,
        IDateTimeProvider dateTimeProvider,
        IOrganisationsRepository organisationsRepository
        ) : base(logger)
    {
        this._dateTimeProvider = dateTimeProvider;
        _organisationsRepository = organisationsRepository;
    }

    /// <summary>
    /// Handles the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<List<LicenceHolderLinkDto>>> Handle(GetLicenceHolderLinksQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.GetById(request.OrganisationId, includeLicenceHolderLinks: true);
        if (organisation == null)
        {
            return CannotFindOrganisation(request.OrganisationId);
        }

        var now = _dateTimeProvider.UtcDateNow;
        return organisation.GetOngoingLicenceHolderLinks(now);
    }
}