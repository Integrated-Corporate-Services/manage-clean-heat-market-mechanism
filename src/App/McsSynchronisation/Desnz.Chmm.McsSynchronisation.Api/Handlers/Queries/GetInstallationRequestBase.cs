using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;

/// <summary>
/// Provides common functionality for MCS data synchronisation
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class GetInstallationRequestBase<TRequest, TResponse> : BaseRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IInstallationRequestRepository _installationRequestRepository;
    private readonly IMcsReferenceDataRepository _referenceDataRepository;

    /// <summary>
    /// Provides common functionality for MCS data synchronisation
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    protected GetInstallationRequestBase(
            ILogger<BaseRequestHandler<TRequest, TResponse>> logger,
            IInstallationRequestRepository installationRequestRepository,
            IMcsReferenceDataRepository referenceDataRepository) : base(logger)
    {
        _installationRequestRepository = installationRequestRepository;
        _referenceDataRepository = referenceDataRepository;
    }

    protected async Task<List<InstallationRequestSummaryDto>> GetInstallationSummaries()
    {
        return await GetInstallationSummaries(x => true);
    }

    protected async Task<List<InstallationRequestSummaryDto>> GetInstallationSummaries(SchemeYearDto schemeYear)
    {
        var schemeYearStartDate = schemeYear.StartDate.ToDateTime(new TimeOnly(0, 0, 0));
        var schemeYearEndDate = schemeYear.EndDate.ToDateTime(new TimeOnly(0, 0, 0));

        return await GetInstallationSummaries(x =>
            (schemeYearStartDate <= x.StartDate && x.StartDate <= schemeYearEndDate) ||
            (schemeYearStartDate <= x.EndDate && x.EndDate <= schemeYearEndDate)
        );
    }

    private async Task<List<InstallationRequestSummaryDto>> GetInstallationSummaries(Expression<Func<InstallationRequest, bool>> filter)
    {
        var technologyTypes = await _referenceDataRepository.GetAllTechnologyTypes();
        var newBuildOptions = await _referenceDataRepository.GetAllNewBuildOptions();
        var installs = await _installationRequestRepository.GetAll(filter);
        return installs
            .OrderByDescending(i => i.EndDate)
            .Select(i => i.ToSummaryDto(technologyTypes, newBuildOptions))
            .ToList();
    }
}
