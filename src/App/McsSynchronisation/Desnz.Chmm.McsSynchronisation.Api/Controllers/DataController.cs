using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.McsSynchronisation.Api.Controllers
{
    /// <summary>
    /// Provides access to the raw MCS data
    /// </summary>
    [Route("api/mcssynchronisation/data")]
    public class DataController : Controller
    {
        private readonly ILogger<DataController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// DI constructor
        /// </summary>
        /// <param name="logger">System logger</param>
        /// <param name="mediator">Mediator</param>
        public DataController(
            ILogger<DataController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Get a list of installation request summaries detailing all the requests that have been made to retrieve MCS data
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A summary list of MCS data requestse</returns>
        [HttpGet("requests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Admins)]
        public async Task<ActionResult<List<InstallationRequestSummaryDto>>> GetInstallationRequests(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetAllInstallationRequestsQuery(), cancellationToken);
        }

        [HttpGet("requests/year/{schemeYearId:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.AdminsAndManufacturer)]
        public async Task<ActionResult<List<InstallationRequestSummaryDto>>> GetInstallationRequests(Guid schemeYearId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetInstallationRequestsQuery(schemeYearId), cancellationToken);
        }

        /// <summary>
        /// Download a CSV file of the data contianed within the specified MCS synchronisation
        /// </summary>
        /// <param name="requestId">The Id of the sync request to download data for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>CSV file of the data to download</returns>
        [HttpGet("requests/{requestId:guid}/download")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Admins)]
        public async Task<ActionResult<Stream>> DownloadRequestDataFile([FromRoute] Guid requestId, CancellationToken cancellationToken)
        {
            var query = new DownloadRequestDataFileQuery(requestId);
            return await _mediator.Send(query, cancellationToken);
        }
    }
}
