using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries
{
    /// <summary>
    /// Handle the query for collecting a list of installtion requests
    /// </summary>
    public class GetAllInstallationRequestsQueryHandler : GetInstallationRequestBase<GetAllInstallationRequestsQuery, ActionResult<List<InstallationRequestSummaryDto>>>
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="logger">System logger</param>
        /// <param name="installationRequestRepository"></param>
        /// <param name="referenceDataRepository"></param>
        public GetAllInstallationRequestsQueryHandler(
            ILogger<GetAllInstallationRequestsQueryHandler> logger,
            IInstallationRequestRepository installationRequestRepository,
            IMcsReferenceDataRepository referenceDataRepository) : base(logger, installationRequestRepository, referenceDataRepository)
        {
        }

        /// <summary>
        /// Query handler returning a list of installation request summaries
        /// </summary>
        /// <param name="request">Query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of installation request summaries</returns>
        public override async Task<ActionResult<List<InstallationRequestSummaryDto>>> Handle(GetAllInstallationRequestsQuery request, CancellationToken cancellationToken)
        {
            return await GetInstallationSummaries();
        }
    }
}
