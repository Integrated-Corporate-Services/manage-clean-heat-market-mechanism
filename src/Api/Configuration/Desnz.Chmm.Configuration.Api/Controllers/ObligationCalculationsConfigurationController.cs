using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Configuration.Api.Controllers
{
    /// <summary>
    /// Handles configuration data for credit weightings
    /// </summary>
    [ApiController]
    [Route("api/configuration/obligationcalculations")]
    [Authorize(Roles = Roles.Everyone)]
    public class ObligationCalculationsConfigurationController : Controller
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mediator">Mediator</param>
        public ObligationCalculationsConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Return the credit weightings for a given scheme year
        /// </summary>
        /// <param name="schemeYearId">The scheme year</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">The credit weightings for the given year</response>
        /// <response code="400">Failed to load the scheme year</response>
        [HttpGet("{schemeYearId:guid}")]
        public async Task<ActionResult<ObligationCalculationsDto>> GetObligationCalculations(Guid schemeYearId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetObligationCalculationsQuery(schemeYearId), cancellationToken);
        }
    }
}