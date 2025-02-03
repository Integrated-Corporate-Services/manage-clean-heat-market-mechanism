using Desnz.Chmm.CreditLedger.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.CreditLedger.Api.Services;
using AutoMapper;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

/// <summary>
/// Test handler for calculating installations credits
/// </summary>
public class CalculateCreditsTestCommandHandler : BaseRequestHandler<CalculateCreditsTestCommand, ActionResult<List<HeatPumpInstallationDto>>>
{
    private readonly IMapper _mapper;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;
    private readonly ISchemeYearService _schemeYearService;


    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Provides access to logging</param>
    /// <param name="mapper"></param>
    /// <param name="installationCreditCalculator"></param>
    /// <param name="schemeYearService"></param>
    public CalculateCreditsTestCommandHandler(
        ILogger<BaseRequestHandler<CalculateCreditsTestCommand, ActionResult<List<HeatPumpInstallationDto>>>> logger,
        IMapper mapper,
        ICreditLedgerCalculator installationCreditCalculator,
        ISchemeYearService schemeYearService
        ) : base(logger)
    {
        _mapper = mapper;
        _installationCreditCalculator = installationCreditCalculator;
        _schemeYearService = schemeYearService;
    }

    /// <summary>
    /// Handles a submission of quarterly boiler sales data
    /// </summary>  
    /// <param name="command">Submission details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of boiler sales data</returns>
    public override async Task<ActionResult<List<HeatPumpInstallationDto>>> Handle(CalculateCreditsTestCommand command, CancellationToken cancellationToken)
    {
        var yearResponse = await _schemeYearService.GetCurrentSchemeYear(cancellationToken);
        if (!yearResponse.IsSuccessStatusCode || yearResponse.Result == null)
            return CannotLoadCurrentSchemeYear(yearResponse.Problem);

        var schemeYear = yearResponse.Result;

        var installations = _mapper.Map<List<CalculatedInstallationCreditDto>>(command.Installations);

        var weightings = await _schemeYearService.GetCreditWeightings(schemeYear.Id, cancellationToken);
        var weightingDictionary = weightings.Result.ToWeightingDictionary();

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };

        Parallel.ForEach(installations, parallelOptions, (installation, token) =>
        {
            installation.Credit = _installationCreditCalculator.Calculate(installation, weightingDictionary);
        });

        var list = _mapper.Map(installations, command.Installations).ToList();
        return list;
    }
}