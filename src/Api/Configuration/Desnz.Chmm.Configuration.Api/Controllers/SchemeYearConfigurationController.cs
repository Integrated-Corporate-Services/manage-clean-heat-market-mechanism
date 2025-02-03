using Desnz.Chmm.Configuration.Common.Commands;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Configuration.Api.Controllers
{
    /// <summary>
    /// Handles all configuration data for a scheme year
    /// </summary>
    [ApiController]
    [Route("api/configuration/schemeyear")]
    [Authorize(Roles = Roles.Everyone)]
    public class SchemeYearConfigurationController : Controller
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mediator">Mediator</param>
        public SchemeYearConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Return scheme year configuraiton data for the given year
        /// </summary>
        /// <param name="schemeYearId">The scheme year</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the given year</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("{schemeYearId:guid}")]
        public async Task<ActionResult<SchemeYearDto>> GetSchemeYear(Guid schemeYearId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetSchemeYearQuery(schemeYearId), cancellationToken);
        }

        /// <summary>
        /// Return scheme year configuraiton data for the year that comes after the year Id provided
        /// </summary>
        /// <param name="schemeYearId">The scheme year</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the given year</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("{schemeYearId:guid}/next")]
        public async Task<ActionResult<SchemeYearDto>> GetNextSchemeYear(Guid schemeYearId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetNextSchemeYearQuery(schemeYearId), cancellationToken);
        }

        /// <summary>
        /// Return scheme year configuraiton data for the current
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the current</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("current")]
        public async Task<ActionResult<SchemeYearDto>> GetCurrentSchemeYear(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetCurrentSchemeYearQuery(), cancellationToken);
        }

        /// <summary>
        /// Return scheme year configuraiton data for the current
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the current</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("current/surrender-day")]
        public async Task<ActionResult<SchemeYearDto>> GetCurrentSchemeYearBySurrenderDay(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetCurrentSchemeYearBySurrenderDayQuery(), cancellationToken);
        }

        /// <summary>
        /// Return scheme year quarter configuraiton data for the given year
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the current quarter</response>
        /// <response code="400">Failed to load the scheme year or quarter</response>
        [HttpGet("quarter/current")]
        public async Task<ActionResult<SchemeYearQuarterDto>> GetCurrentSchemeYearQuarter(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetCurrentSchemeYearQuarterQuery(), cancellationToken);
        }

        /// <summary>
        /// Return scheme year configuraiton data for all years
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for all years</response>
        [HttpGet("all")]
        public async Task<ActionResult<List<SchemeYearDto>>> GetAllSchemeYears(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetAllSchemeYearsQuery(), cancellationToken);
        }

        /// <summary>
        /// Generate the next years scheme year configuration data
        /// </summary>
        /// <param name="command">Command specifying the previous scheme year ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">The scheme year id</response>
        /// <response code="404">Scheme year not found</response>
        /// <response code="400">Scheme year already exists</response>
        [HttpPost]
        [Authorize(Roles = Roles.ApiRole)]
        public async Task<ActionResult<Guid>> GenerateNextYearsScheme([FromBody] GenerateNextYearsSchemeaCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// Generate the next years scheme year configuration data
        /// </summary>
        /// <param name="command">Command specifying the previous scheme year ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">The scheme year id</response>
        /// <response code="404">Scheme year not found</response>
        [HttpPost("rollback")]
        [Authorize(Roles = Roles.ApiRole)]
        public async Task<ActionResult<Guid>> RollbackGenerateNextYearsScheme([FromBody] RollbackGenerateNextYearsSchemeCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// Return scheme year configuraiton data for the summary screen for the given year
        /// </summary>
        /// <param name="schemeYearId">The scheme year</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The configuration data for the given year</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("{schemeYearId:guid}/summary")]
        public async Task<ActionResult<SchemeYearSummaryConfigurationDto>> GetSchemeYearSummaryConfiguration(Guid schemeYearId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetSchemeYearSummaryConfigurationQuery(schemeYearId), cancellationToken);
        }

        /// <summary>
        /// Return the first scheme year
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The first scheme year or null</response>
        [HttpGet("first")]
        public async Task<ActionResult<SchemeYearDto>> GetFirstSchemeYear(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetFirstSchemeYearQuery(), cancellationToken);
        }

        /// <summary>
        /// Update scheme year configuration
        /// </summary>
        /// <param name="schemeYearId">The scheme year</param>
        /// <param name="command">Configuration data to modify</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Successfully updated scheme year configuration</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpPut("configuration")]
        [Authorize(Roles = Roles.AdminsAndApi)]
        public async Task<ActionResult> UpdateSchemeYearConfiguration([FromBody] UpdateSchemeYearConfigurationCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}