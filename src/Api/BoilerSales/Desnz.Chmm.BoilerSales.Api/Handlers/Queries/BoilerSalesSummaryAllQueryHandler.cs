using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;


namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries
{
    /// <summary>
    /// Handles fetching of the annual and quarterly boiles sales summary
    /// </summary>
    public class BoilerSalesSummaryAllQueryHandler : BaseRequestHandler<BoilerSalesSummaryAllQuery, ActionResult<List<BoilerSalesSummaryDto>>>
    {
        private ISchemeYearService _schemeYearService;
        private IRequestValidator _requestValidator;
        private IBoilerSalesCalculator _boilerSalesCalculator;
        private IAnnualBoilerSalesRepository _annualBoilerSalesRepository;
        private IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;


        public BoilerSalesSummaryAllQueryHandler(
            ILogger<BaseRequestHandler<BoilerSalesSummaryAllQuery, ActionResult<List<BoilerSalesSummaryDto>>>> logger,
            ISchemeYearService schemeYearService,
            IAnnualBoilerSalesRepository annualBoilerSalesRepository,
            IQuarterlyBoilerSalesRepository quarterlyBoilerSalesRepository,
            IBoilerSalesCalculator boilerSalesCalculator,
            IRequestValidator requestValidator) : base(logger)
        {
            _annualBoilerSalesRepository = annualBoilerSalesRepository;
            _quarterlyBoilerSalesRepository = quarterlyBoilerSalesRepository;
            _schemeYearService = schemeYearService;
            _requestValidator = requestValidator;
            _boilerSalesCalculator = boilerSalesCalculator;
        }

        /// <summary>
        /// Handles fetching of the annual and quarterly boiles sales summary
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task<ActionResult<List<BoilerSalesSummaryDto>>> Handle(BoilerSalesSummaryAllQuery query, CancellationToken cancellationToken)
        {
            var schemeYearId = query.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var obligationCalculationsResponse = await _schemeYearService.GetObligationCalculations(query.SchemeYearId, cancellationToken);
            if (!obligationCalculationsResponse.IsSuccessStatusCode || obligationCalculationsResponse.Result == null)
                return CannotLoadObligationCalculations(query.SchemeYearId, obligationCalculationsResponse.Problem);
            var obligationCalculations = obligationCalculationsResponse.Result;

            var allAnnualSales = await _annualBoilerSalesRepository.GetAll(s => s.SchemeYearId == query.SchemeYearId);
            var quarterlySales = await _quarterlyBoilerSalesRepository.GetAllNonAnnual(query.SchemeYearId);

            var orgIds = allAnnualSales.Select(i => i.OrganisationId).Distinct().ToList();
            orgIds.AddRange(quarterlySales.Select(i => i.OrganisationId).Distinct().ToList());

            var data = orgIds.Distinct().Select(o =>
                _boilerSalesCalculator.GenerateBoilerSalesSummary(
                    o, 
                    allAnnualSales.SingleOrDefault(i => i.OrganisationId == o), 
                    quarterlySales.Where(i => i.OrganisationId == o).ToList(), 
                    obligationCalculations)).ToList();

            return data;
        }
    }
}
