using AutoMapper;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Quarterly;

/// <summary>
/// Handles requets for quarterly boiler sale
/// </summary>
public class GetQuarterlyBoilerSalesQueryHandler : BaseRequestHandler<GetQuarterlyBoilerSalesQuery, ActionResult<List<QuarterlyBoilerSalesDto>>>
{
    private readonly IMapper _mapper;
    private readonly IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;
    private readonly ISchemeYearService _schemeYearService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="mapper">Used to map between DTOs and entities</param>
    /// <param name="quarterlyBoilerSalesRepository">Repository providing access to quarterly boiler sales data</param>
    /// <param name="schemeYearService">Used to access Scheme Year data</param>
    public GetQuarterlyBoilerSalesQueryHandler(
        ILogger<BaseRequestHandler<GetQuarterlyBoilerSalesQuery, ActionResult<List<QuarterlyBoilerSalesDto>>>> logger,
        IMapper mapper,
        IQuarterlyBoilerSalesRepository quarterlyBoilerSalesRepository,
        ISchemeYearService schemeYearService,
        IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator) : base(logger)
    {
        _mapper = mapper;
        _quarterlyBoilerSalesRepository = quarterlyBoilerSalesRepository;
        _schemeYearService = schemeYearService;
        _dateTimeProvider = dateTimeProvider;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles a request for quarterly boiler sales data
    /// </summary>
    /// <param name="query">Details of required boiler sales data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Requested boiler sales data</returns>
    public override async Task<ActionResult<List<QuarterlyBoilerSalesDto>>> Handle(GetQuarterlyBoilerSalesQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId);
        if (validationError != null)
            return validationError;

        var yearResponse = await _schemeYearService.GetSchemeYear(schemeYearId, CancellationToken.None);
        if (!yearResponse.IsSuccessStatusCode || yearResponse.Result == null)
            return CannotLoadSchemeYear(schemeYearId, yearResponse.Problem);
        var schemeYear = yearResponse.Result;

        var quarterlyBoilerSales = _mapper.Map<List<QuarterlyBoilerSalesDto>>(
            await _quarterlyBoilerSalesRepository.Get(o =>
                o.OrganisationId == query.OrganisationId && (
                    o.SchemeYearQuarterId == schemeYear.QuarterOne.Id ||
                    o.SchemeYearQuarterId == schemeYear.QuarterTwo.Id ||
                    o.SchemeYearQuarterId == schemeYear.QuarterThree.Id ||
                    o.SchemeYearQuarterId == schemeYear.QuarterFour.Id
                ), includeFiles: true, includeChanges: true));

        QuarterlyBoilerSalesDto GetDefaultQuarter(SchemeYearQuarterDto schemeYearQuarter) => new()
        {
            OrganisationId = query.OrganisationId.Value,
            SchemeYearQuarterId = schemeYearQuarter.Id.Value,
            Changes = new(),
            Files = new(),
            Status = _dateTimeProvider.IsNowWithinRange(schemeYearQuarter.EndDate, schemeYear.BoilerSalesSubmissionEndDate, DateRange.Exclusive) ? BoilerSalesStatus.Due : BoilerSalesStatus.Default
        };

        return new List<QuarterlyBoilerSalesDto>
        {
            quarterlyBoilerSales.FirstOrDefault(o => o.SchemeYearQuarterId == schemeYear.QuarterOne.Id) ?? GetDefaultQuarter(schemeYear.QuarterOne),
            quarterlyBoilerSales.FirstOrDefault(o => o.SchemeYearQuarterId == schemeYear.QuarterTwo.Id) ?? GetDefaultQuarter(schemeYear.QuarterTwo),
            quarterlyBoilerSales.FirstOrDefault(o => o.SchemeYearQuarterId == schemeYear.QuarterThree.Id) ?? GetDefaultQuarter(schemeYear.QuarterThree),
            quarterlyBoilerSales.FirstOrDefault(o => o.SchemeYearQuarterId == schemeYear.QuarterFour.Id) ?? GetDefaultQuarter(schemeYear.QuarterFour)
        };
    }
}
