using AutoMapper;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Commands;

/// <summary>
/// Handler for generating credits for the entire requested installations lot
/// </summary>
public class GenerateCreditsCommandHandler : BaseRequestHandler<GenerateCreditsCommand, ActionResult>
{
    private readonly ILogger<BaseRequestHandler<GenerateCreditsCommand, ActionResult>> _logger;
    private readonly IMapper _mapper;
    private readonly IInstallationCreditRepository _installationCreditRepository;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly ICreditLedgerCalculator _installationCreditCalculator;
    private readonly ISchemeYearService _schemeYearService;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="logger">Provides access to logging</param>
    /// <param name="mapper"></param>
    /// <param name="installationCreditRepository">Repository used to read/write boiler sales data</param>
    /// <param name="licenceHolderService"></param>
    /// <param name="installationCreditCalculator"></param>
    /// <param name="schemeYearService"></param>
    public GenerateCreditsCommandHandler(ILogger<BaseRequestHandler<GenerateCreditsCommand, ActionResult>> logger,
                                                   IMapper mapper,
                                                   IInstallationCreditRepository installationCreditRepository,
                                                   ILicenceHolderService licenceHolderService,
                                                   ICreditLedgerCalculator installationCreditCalculator,
                                                   ISchemeYearService schemeYearService) : base(logger)
    {
        _logger = logger;
        _mapper = mapper;

        _installationCreditRepository = installationCreditRepository;
        _licenceHolderService = licenceHolderService;
        _installationCreditCalculator = installationCreditCalculator;
        _schemeYearService = schemeYearService;
    }

    /// <summary>
    /// Handles generating credits for the entire requested installations lot
    /// </summary>  
    /// <param name="command">Submission details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of boiler sales data</returns>
    public override async Task<ActionResult> Handle(GenerateCreditsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - GenerateCreditsCommandHandler ({command.Installations.Count()}) - Started");

        var httpResponseLicenceHolders = await _licenceHolderService.GetAll();
        if (!httpResponseLicenceHolders.IsSuccessStatusCode || httpResponseLicenceHolders.Result == null)
            return CannotLoadLicenceHolders(httpResponseLicenceHolders.Problem);
        var licenceHolders = httpResponseLicenceHolders.Result.ToDictionary(x => x.McsManufacturerId, y => y.Id);

        var weightings = await _schemeYearService.GetAllCreditWeightings(cancellationToken);
        if (!weightings.IsSuccessStatusCode || weightings.Result == null)
            return CannotLoadCreditWeightings(weightings.Problem);
        var weightingDictionaries = weightings.Result.Select(i => i.ToWeightingDictionary());

        var schemeYearsResponse = await _schemeYearService.GetAllSchemeYears(cancellationToken);
        if (!schemeYearsResponse.IsSuccessStatusCode || schemeYearsResponse.Result == null)
            return CannotLoadAllSchemeYears(schemeYearsResponse.Problem);

        Configuration.Common.Dtos.SchemeYearDto? GetSchemeYear(DateTime? commissioningDate)
        {
            return schemeYearsResponse.Result?.FirstOrDefault(x =>
                x.CreditGenerationWindowStartDate <= DateOnly.FromDateTime(commissioningDate ?? DateTime.MinValue) &&
                x.CreditGenerationWindowEndDate >= DateOnly.FromDateTime(commissioningDate ?? DateTime.MinValue));
        };

        var installations = _mapper.Map<List<CalculatedInstallationCreditDto>>(command.Installations.Where(x => GetSchemeYear(x.CommissioningDate) != null));

        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - Fetching credit calculation data ({installations.Count}) - Ended");

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };
        Parallel.ForEach(installations, parallelOptions, (installation, token) =>
        {
            var schemeYear = GetSchemeYear(installation.CommissioningDate)!;
            installation.SchemeYearId = schemeYear.Id;
            installation.Credit = _installationCreditCalculator.Calculate(installation, weightingDictionaries.Single(i => i.SchemeYearId == schemeYear.Id));
        });

        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - Calculating Installations Credits ({installations.Count}) - Ended");

        var installationCredits = new ConcurrentBag<InstallationCredit>();

        Parallel.ForEach(installations.Where(x => x.SchemeYearId.HasValue), parallelOptions, (installation, token) =>
        {
            installationCredits.AddRange(GetCreditsByManufacturer(installation, licenceHolders));
        });

        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - Adding Installations Credits ({installationCredits.Count}) - Ended");

        await _installationCreditRepository.Append(installationCredits);

        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - Persisting Installations Credits ({installationCredits.Count}) - Started");
        await _installationCreditRepository.UnitOfWork.SaveChangesAsync();
        _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} - Persisting Installations Credits ({installationCredits.Count}) - Ended");

        return Responses.Ok();
    }

    protected IList<InstallationCredit> GetCreditsByManufacturer(CalculatedInstallationCreditDto installation, Dictionary<int, Guid> licenceHolders)
    {
        var list =  installation.HeatPumpProducts.GroupBy(x => x.ManufacturerId)
                                                 .Select(mcsProductDto => new InstallationCredit(licenceHolders[mcsProductDto.Key],
                                                                                                   installation.MidId.Value,
                                                                                                   installation.SchemeYearId.Value,
                                                                                                   DateOnly.FromDateTime(installation.CommissioningDate.Value),
                                                                                                   installation.Credit * mcsProductDto.Count(),
                                                                                                   installation.IsHybrid ?? false)).ToList();

        return list;
    }
}