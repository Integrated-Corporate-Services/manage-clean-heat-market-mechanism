using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries
{
    /// <summary>
    /// Handle the query for collecting a list of installtion requests
    /// </summary>
    public class GetInstallationRequestsQueryHandler : GetInstallationRequestBase<GetInstallationRequestsQuery, ActionResult<List<InstallationRequestSummaryDto>>>
    {
        private readonly ISchemeYearService _schemeYearService;

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="logger">System logger</param>
        /// <param name="schemeYearService"></param>
        /// <param name="installationRequestRepository"></param>
        /// <param name="referenceDataRepository"></param>
        public GetInstallationRequestsQueryHandler(
            ILogger<GetInstallationRequestsQueryHandler> logger,
            ISchemeYearService schemeYearService,
            IInstallationRequestRepository installationRequestRepository,
            IMcsReferenceDataRepository referenceDataRepository) : base(logger, installationRequestRepository, referenceDataRepository)
        {
            _schemeYearService = schemeYearService;
        }

        /// <summary>
        /// Query handler returning a list of installation request summaries
        /// </summary>
        /// <param name="request">Query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of installation request summaries</returns>
        public override async Task<ActionResult<List<InstallationRequestSummaryDto>>> Handle(GetInstallationRequestsQuery request, CancellationToken cancellationToken)
        {
            var schemeYearHttpResponse = await _schemeYearService.GetSchemeYear(request.SchemeYearId, cancellationToken);
            if (!schemeYearHttpResponse.IsSuccessStatusCode || schemeYearHttpResponse.Result == null)
                return CannotLoadSchemeYear(request.SchemeYearId, schemeYearHttpResponse.Problem);
            var schemeYear = schemeYearHttpResponse.Result;

            return await GetInstallationSummaries(schemeYear);
        }
    }
}
