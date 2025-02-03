using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetLicenceHolderLinksHistoryQueryHandler : BaseRequestHandler<GetLicenceHolderLinksHistoryQuery, ActionResult<List<LicenceHolderLinkDto>>>
{
    private readonly IOrganisationsRepository _organisationsRepository;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public GetLicenceHolderLinksHistoryQueryHandler(
        ILogger<BaseRequestHandler<GetLicenceHolderLinksHistoryQuery, ActionResult<List<LicenceHolderLinkDto>>>> logger,
        IOrganisationsRepository organisationsRepository
        ) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
    }

    /// <summary>
    /// Handles the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<List<LicenceHolderLinkDto>>> Handle(GetLicenceHolderLinksHistoryQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.GetById(request.OrganisationId, includeLicenceHolderLinks: true);
        if (organisation == null)
        {
            return CannotFindOrganisation(request.OrganisationId);
        }
        return organisation.GetOngoingLicenceHolderLinksHistory()
            .OrderByDescending(i => i.EndDate, new DateOnlyDescendingNullFirstComparer())
            .ThenBy(i => i.StartDate).ToList();
    }

    public class DateOnlyDescendingNullFirstComparer : IComparer<DateOnly?>
    {
        public int Compare(DateOnly? x, DateOnly? y)
        {
            if (!x.HasValue && !y.HasValue)
                return 0;
            else if (!x.HasValue)
                return 1;
            else if (!y.HasValue)
                return -1;
            else
                return y.Value.CompareTo(x.Value);
        }
    }
}