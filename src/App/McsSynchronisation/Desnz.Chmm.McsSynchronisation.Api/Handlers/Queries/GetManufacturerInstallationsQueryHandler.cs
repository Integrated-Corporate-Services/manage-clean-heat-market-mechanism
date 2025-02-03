using AutoMapper;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
/// <summary>
/// Handles retrieving manufacturer installations
/// </summary>
public class GetManufacturerInstallationsQueryHandler : BaseRequestHandler<GetManufacturerInstallationsQuery, ActionResult<List<CreditCalculationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IHeatPumpInstallationsRepository _heatPumpInstallationsRepository;
    private readonly ILicenceHolderService _licenceHolderService;

    /// <summary>
    /// Constructs the handler
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="heatPumpInstallationsRepository"></param>
    /// <param name="licenceHolderService"></param>
    public GetManufacturerInstallationsQueryHandler(
        ILogger<BaseRequestHandler<GetManufacturerInstallationsQuery, ActionResult<List<CreditCalculationDto>>>> logger,
        IMapper mapper,
        IHeatPumpInstallationsRepository heatPumpInstallationsRepository,
        ILicenceHolderService licenceHolderService) : base(logger)
    {
        _mapper = mapper;
        _heatPumpInstallationsRepository = heatPumpInstallationsRepository;
        _licenceHolderService = licenceHolderService;
    }

    /// <summary>
    /// Retrieves manufacturer installations
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<ActionResult<List<CreditCalculationDto>>> Handle(GetManufacturerInstallationsQuery command, CancellationToken cancellationToken)
    {
        try
        {
            var httpResponseLicenceHolders = await _licenceHolderService.GetAll();
            if (!httpResponseLicenceHolders.IsSuccessStatusCode || httpResponseLicenceHolders.Result == null)
                return CannotLoadLicenceHolders(httpResponseLicenceHolders.Problem);

            var licenceHolders = httpResponseLicenceHolders.Result.ToDictionary(x => x.McsManufacturerId, y => y.Id);
            if (!licenceHolders.ContainsKey(command.ManufacturerId))
                return LicenceHolderNotReturned(command.ManufacturerId, httpResponseLicenceHolders.Result.Select(i => i.Id));

            var installations = await _heatPumpInstallationsRepository.GetAll(x => x.CommissioningDate >= command.StartDate &&
                                                                                      x.CommissioningDate <= command.EndDate.AddDays(1).AddTicks(-1) &&
                                                                                      x.HeatPumpProducts.All(x => x.ManufacturerId == command.ManufacturerId));

            var installationDtos = _mapper.Map<List<CreditCalculationDto>>(installations);

            return installationDtos;
        }
        catch (Exception ex)
        {
            return ErrorGettingInstallations(ex);
        }
    }
}