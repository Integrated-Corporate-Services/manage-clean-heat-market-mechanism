using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.CreditLedger.Api.Controllers;

[ApiController]
[Route("api/creditledger")]
[Authorize]
public class CreditLedgerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreditLedgerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("installation-request/generate-credits")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> GenerateCredits([FromBody] GenerateCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("calculate-credits")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<HeatPumpInstallationDto>>> CalculateCreditsTestEndpoint([FromBody] CalculateCreditsTestCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Provides access to the current credit blanace for the provided organisation
    /// </summary>
    /// <param name="organisationId">The Id of the organisation being queried</param>
    /// <param name="schemeYearId">The scheme year to interrogate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpGet("year/{schemeYearId:guid}/credit-balances")]
    [Authorize(Roles = Roles.AdminsAndApi)]
    public async Task<ActionResult<List<OrganisationCreditBalanceDto>>> GetAllCreditBalances(Guid schemeYearId, CancellationToken cancellationToken)
    {
        var query = new GetAllCreditBalancesQuery(schemeYearId);

        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Transfer credits between organisations
    /// </summary>
    /// <param name="command">Details of the transfer</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPost("transfer-credits")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult> TransferCredits([FromBody] TransferCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Adjust credits of an organisation
    /// </summary>
    /// <param name="command">Details of the adjustment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPost("adjust-credits")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> AdjustCredits([FromBody] AdjustCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Get the scheme year summary of transactions
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Scheme year summary for the credit ledger</returns>
    [HttpGet]
    [Route("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/summary")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<CreditLedgerSummaryDto>> GetCreditLedgerSummary([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCreditLedgerSummaryQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Get the scheme year transactions for tranfer in / out
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit Transfers for the given year</returns>
    [HttpGet]
    [Route("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/transfers")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<CreditLedgerTransfersDto>> GetCreditLedgerTransfers([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCreditLedgerTransfersQuery(organisationId, schemeYearId), cancellationToken);
    }

    [HttpPost("redeem")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> LogRedemption([FromBody] RedeemCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("redeem/rollback")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> RollbackRedemption([FromBody] RollbackRedeemCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("carryover/rollback")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> RollbackCarryOverCredit([FromBody] RollbackCarryOverCreditCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("carryover")]
    [Authorize(Roles = Roles.ApiRole)]
    public async Task<ActionResult> CarryOverCredits([FromBody] CarryOverCreditsCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPost("carryover-newlicenceholders")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult> CarryOverNewLicenceHolders([FromBody] CarryOverNewLicenceHoldersCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Provides access to the current credit blanace for the provided organisation
    /// </summary>
    /// <param name="organisationId">The Id of the organisation being queried</param>
    /// <param name="schemeYearId">The scheme year to interrogate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpGet("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/credit-balance")]
    [Authorize(Policy = AuthorizationConstants.CanAccessOrganisation, Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<CreditBalanceDto>> CreditBalance(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        var query = new CreditBalanceQuery(organisationId, schemeYearId);

        return await _mediator.Send(query, cancellationToken);
    }

    [HttpGet("period/{startDate}/{endDate}")]
    [Authorize(Roles = Roles.Admins)]
    public async Task<ActionResult<List<HeatPumpInstallationCreditsDto>>> GetInstallationCredits(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var query = new GetInstallationCreditsQuery(startDate, endDate);

        return await _mediator.Send(query, cancellationToken);
    }

    [HttpGet]
    [Route("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/credit-totals")]
    [Authorize(Roles = Roles.AdminsAndManufacturer)]
    public async Task<ActionResult<List<PeriodCreditTotals>>> GetManufacturerCreditTotals(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        var query = new GetManufacturerCreditTotalsQuery(organisationId, schemeYearId);

        return await _mediator.Send(query, cancellationToken);
    }
}
