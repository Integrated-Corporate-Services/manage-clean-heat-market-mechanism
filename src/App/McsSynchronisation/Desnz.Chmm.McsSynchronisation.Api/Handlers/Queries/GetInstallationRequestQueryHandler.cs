using AutoMapper;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
/// <summary>
/// Handles retrieving manufacturer installations
/// </summary>
public class GetInstallationRequestQueryHandler : BaseRequestHandler<GetInstallationRequestQuery, ActionResult<DataPage<CreditCalculationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IHeatPumpInstallationsRepository _heatPumpInstallationsRepository;

    /// <summary>
    /// Constructs the handler
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="heatPumpInstallationsRepository"></param>
    public GetInstallationRequestQueryHandler(
        ILogger<BaseRequestHandler<GetInstallationRequestQuery, ActionResult<DataPage<CreditCalculationDto>>>> logger,
        IMapper mapper,
        IHeatPumpInstallationsRepository heatPumpInstallationsRepository) : base(logger)
    {
        _mapper = mapper;
        _heatPumpInstallationsRepository = heatPumpInstallationsRepository;
    }

    /// <summary>
    /// Retrieves manufacturer installations
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<ActionResult<DataPage<CreditCalculationDto>>> Handle(GetInstallationRequestQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var installations = await _heatPumpInstallationsRepository.GetAll(x => x.InstallationRequestId == query.InstallationRequestId);

            var installationDtos = _mapper.Map<List<CreditCalculationDto>>(installations);

            return installationDtos.ToPage(1000, query.PageNumber);
        }
        catch (Exception ex)
        {
            return ErrorGettingInstallations(query.InstallationRequestId, ex);
        }
    }
}