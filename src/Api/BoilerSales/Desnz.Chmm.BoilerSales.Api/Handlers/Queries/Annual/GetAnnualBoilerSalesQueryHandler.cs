using AutoMapper;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual;

/// <summary>
/// Handler for retrieval of annual boiler sales data
/// </summary>
public class GetAnnualBoilerSalesQueryHandler : BaseRequestHandler<GetAnnualBoilerSalesQuery, ActionResult<AnnualBoilerSalesDto>>
{
    private readonly IMapper _mapper;
    private readonly IAnnualBoilerSalesRepository _annualBoilerSalesRepository;
    private readonly ISchemeYearService _schemeYearService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor accepting all dependencies
    /// </summary>
    /// <param name="mapper">Used to map between DTOs and entities</param>
    /// <param name="annualBoilerSalesRepository">Repository providing read/write access to boiler sales data</param>
    /// <param name="userService">Provides access to the current user</param>
    /// <param name="schemeYearService">Provides access to Scheme Year data</param>
    /// <param name="dateTimeProvider"></param>
    public GetAnnualBoilerSalesQueryHandler(
        ILogger<BaseRequestHandler<GetAnnualBoilerSalesQuery, ActionResult<AnnualBoilerSalesDto>>> logger,
        IMapper mapper,
        IAnnualBoilerSalesRepository annualBoilerSalesRepository,
        ISchemeYearService schemeYearService,
        IDateTimeProvider dateTimeProvider,
        IRequestValidator requestValidator) : base(logger)
    {
        _mapper = mapper;
        _annualBoilerSalesRepository = annualBoilerSalesRepository;
        _schemeYearService = schemeYearService;
        _dateTimeProvider = dateTimeProvider;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles requets for annual boiler sales data
    /// </summary>
    /// <param name="query">Details of query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Annual boiler sales data for the given Scheme Year</returns>
    public override async Task<ActionResult<AnnualBoilerSalesDto>> Handle(GetAnnualBoilerSalesQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var annualBoilerSales = await _annualBoilerSalesRepository.Get(
            o => 
                o.OrganisationId == query.OrganisationId && 
                o.SchemeYearId == query.SchemeYearId, 
            includeFiles: true, 
            includeChanges: true);

        if (annualBoilerSales == null)
        {
            var schemeYearResponse = await _schemeYearService.GetSchemeYear(query.SchemeYearId, cancellationToken);
            var schemeYear = schemeYearResponse.Result;

            var now = _dateTimeProvider.UtcDateNow;

            return new AnnualBoilerSalesDto
            {
                SchemeYearId = schemeYear.Id,
                OrganisationId = query.OrganisationId,
                Changes = new(),
                Files = new(),
                Status = now > schemeYear.EndDate && now <= schemeYear.BoilerSalesSubmissionEndDate ? BoilerSalesStatus.Due : BoilerSalesStatus.Default
            };
        }

        return _mapper.Map<AnnualBoilerSalesDto>(annualBoilerSales);
    }
}
