using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Api.Services;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using IDateTimeProvider = Desnz.Chmm.Common.Providers.IDateTimeProvider;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Commands;

/// <summary>
/// Provides common functionality for MCS data synchronisation
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class InstallationRequestBase<TRequest, TResponse> : BaseRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<BaseRequestHandler<TRequest, TResponse>> _logger;
    private readonly IMapper _mapper;
    private readonly IMcsInstallationDataRepository _mcsDataRepository;
    private readonly ILicenceHolderService _licenceHolderService;
    private readonly ICreditLedgerService _creditLedgerService;
    private readonly IIdentityService _identityService;
    private readonly ApiKeyPolicyConfig _apiKeyPolicyConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDateTimeProvider _datetimeProvider;
    private readonly IMcsMidService _mcsMidService;

    /// <summary>
    /// Provides common functionality for MCS data synchronisation
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="mcsDataRepository"></param>
    /// <param name="licenceHolderService"></param>
    /// <param name="creditLedgerService"></param>
    /// <param name="identityService"></param>
    /// <param name="apiKeyPolicyConfig"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="datetimeProvider"></param>
    /// <param name="mcsMidService"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected InstallationRequestBase(ILogger<BaseRequestHandler<TRequest, TResponse>> logger,
        IMapper mapper,
        IMcsInstallationDataRepository mcsDataRepository,
        ILicenceHolderService licenceHolderService,
        ICreditLedgerService creditLedgerService,
        IIdentityService identityService,
        IOptions<ApiKeyPolicyConfig> apiKeyPolicyConfig,
        IHttpContextAccessor httpContextAccessor,
        IDateTimeProvider datetimeProvider,
        IMcsMidService mcsMidService) : base(logger)
    {
        _logger = logger;
        _mapper = mapper;
        _mcsDataRepository = mcsDataRepository;
        _licenceHolderService = licenceHolderService;
        _creditLedgerService = creditLedgerService;
        _identityService = identityService;
        _apiKeyPolicyConfig = apiKeyPolicyConfig.Value
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ApiKeyPolicy));
        _httpContextAccessor = httpContextAccessor;
        _datetimeProvider = datetimeProvider;
        _mcsMidService = mcsMidService;
    }

    /// <summary>
    /// Check if installation request already exists for date range
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    protected async Task<bool> DoesAlreadyExist(DateTime startDate, DateTime endDate)
    {
        var existing = await _mcsDataRepository.GetRequest(startDate, endDate);
        return existing != null;
    }

    /// <summary>
    /// manages fetching and persisting MCS data, followed by credits generation
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="technologyTypeIds"></param>
    /// <param name="isNewBuildIds"></param>
    /// <returns></returns>
    protected async Task<ActionResult> SynchroniseInstallations(DateTime startDate, DateTime endDate, int[]? technologyTypeIds, int[]? isNewBuildIds)
    {
        try
        {
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Requesting Installations - Started");
            //1. Retrieve authentication token
            var apiKey = _httpContextAccessor.HttpContext?.Request.Headers[_apiKeyPolicyConfig.HeaderName].FirstOrDefault();
            var response = await _identityService.GetJwtToken(new GetJwtTokenCommand(null, null, null, apiKey));
            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(response.Result))
                return CannotLoadJwtToken(response.Problem);

            //2. Get the MCS data
            var getHeatPumpInstallationsResult = await _mcsMidService.GetHeatPumpInstallations(new GetMcsInstallationsDto(startDate, endDate, technologyTypeIds, isNewBuildIds));
            if (!getHeatPumpInstallationsResult.IsSuccessStatusCode || getHeatPumpInstallationsResult.Result == null)
                return CannotLoadHeatPumpInstallations(startDate, endDate, technologyTypeIds, isNewBuildIds, getHeatPumpInstallationsResult.Problem);

            //if (!getHeatPumpInstallationsResult.Result.Installations.Any())
            //{
            //    return new OkResult();
            //}

            var installationDtos = getHeatPumpInstallationsResult.Result.Installations;
            var productDtos = installationDtos.SelectMany(x => x.HeatPumpProducts).Distinct().ToList();
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Fetched MCS Installations");

            //3. Create Licence Holders
            var licenceHolders = _mapper.Map<List<CreateLicenceHolderCommand>>(productDtos).Distinct();
            var httpResponseLicenceHolders = await _licenceHolderService.Create(new CreateLicenceHoldersCommand { LicenceHolders = licenceHolders }, response.Result);
            if (!httpResponseLicenceHolders.IsSuccessStatusCode)
                return CannotCreateLicenceHolders(httpResponseLicenceHolders.Problem);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Created Licence Holders");

            //4. Create Products
            var products = _mapper.Map<List<HeatPumpProduct>>(productDtos);
            await _mcsDataRepository.AppendProducts(products);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Added Products");

            //5. Create Installation Request
            var installationRequest = new InstallationRequest { RequestDate = _datetimeProvider.UtcNow, StartDate = startDate, EndDate = endDate };
            await _mcsDataRepository.AddInstallationRequests(installationRequest);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Added Installation Request");

            //6. Create Installations
            var installations = _mapper.Map<List<HeatPumpInstallation>>(installationDtos);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };
            Parallel.ForEach(installations, parallelOptions, (installation, token) =>
            {
                installation.InstallationRequestId = installationRequest.Id;
                installation.Id = Guid.NewGuid();
            });
            await _mcsDataRepository.AddInstallations(installations);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Added Installations");

            //7. Create InstallationProducts
            var installationProducts = installationDtos.Select((x, idx) => new { InstallationId = installations[idx].Id, ProductIds = x.HeatPumpProducts.Select(y => y.Id) })
                                            .SelectMany(item => item.ProductIds.Select(id => new HeatPumpInstallationProduct { InstallationId = item.InstallationId, ProductId = id }))
                                            .ToList();

            await _mcsDataRepository.AddInstallationProducts(installationProducts);
            _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Added Installation Products");

            try
            {
                //8. Persist the data context
                await _mcsDataRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Persisted MCS Installation");

                //9. Generate credits for the current request
                if (getHeatPumpInstallationsResult.Result.Installations.Any())
                {
                    HttpObjectResponse<object> httpResponseGenerateCredits = await GenerateCredits(response, installationDtos);

                    if (!httpResponseGenerateCredits.IsSuccessStatusCode)
                        return CannotGenerateCredits(httpResponseGenerateCredits.Problem);
                }

                _logger.LogInformation($"{DateTime.UtcNow.ToString("[dd/MM/yy HH:mm:ss:fff]")} ---> Requesting Installations - Ended");
            }
            catch (Exception ex)
            {
                return ExceptionPersistingMcsData(ex);
            }

            return new OkResult();
        }
        catch (Exception ex)
        {
            return ExceptionPersistingMcsData(ex);
        }
    }

    private async Task<HttpObjectResponse<object>> GenerateCredits(HttpObjectResponse<string> response, List<McsInstallationDto> installationDtos)
    {
        var subListSize = 5000;

        var multipleLists = SplitList(installationDtos, subListSize);

        var bag = new ConcurrentBag<HttpObjectResponse<object>>();
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };
        await Parallel.ForEachAsync(multipleLists, parallelOptions, async (list, token) =>
        {
            bag.Add(await _creditLedgerService.GenerateCredits(new CreditLedger.Common.Commands.GenerateCreditsCommand(list), response.Result));
        });

        if (bag.Any(x => !x.IsSuccessStatusCode))
        {
            var badResponses = bag.Where(x => !x.IsSuccessStatusCode);
            var strings = badResponses.Select(response => JsonConvert.SerializeObject(response.Problem));
            _logger.LogError("Failed to generate credits", string.Join("\r\n", strings));
            return badResponses.First();
        }
        return bag.FirstOrDefault() ?? new HttpObjectResponse<object>(new HttpResponseMessage(System.Net.HttpStatusCode.OK), null);
    }

    private static List<List<McsInstallationDto>> SplitList(List<McsInstallationDto> bigList, int subListSize)
    {
        var list = new List<List<McsInstallationDto>>();

        for (int i = 0; i < bigList.Count; i += subListSize)
            list.Add(bigList.GetRange(i, Math.Min(subListSize, bigList.Count - i)));

        return list;
    }
}