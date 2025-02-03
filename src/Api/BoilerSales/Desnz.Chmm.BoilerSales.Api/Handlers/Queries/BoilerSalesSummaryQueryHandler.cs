using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;


namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries
{
    public class BoilerSalesSummaryQueryHandler : BaseRequestHandler<BoilerSalesSummaryQuery, ActionResult<BoilerSalesSummaryDto>>
    {
        private ISchemeYearService _schemeYearService;
        private IRequestValidator _requestValidator;
        private IBoilerSalesCalculator _boilerSalesCalculator;
        private IAnnualBoilerSalesRepository _annualBoilerSalesRepository;
        private IQuarterlyBoilerSalesRepository _quarterlyBoilerSalesRepository;

        public BoilerSalesSummaryQueryHandler(
            ILogger<BaseRequestHandler<BoilerSalesSummaryQuery, ActionResult<BoilerSalesSummaryDto>>> logger,
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

        public override async Task<ActionResult<BoilerSalesSummaryDto>> Handle(BoilerSalesSummaryQuery query, CancellationToken cancellationToken)
        {
            var organisationId = query.OrganisationId;
            var schemeYearId = query.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var obligationCalculationsResponse = await _schemeYearService.GetObligationCalculations(schemeYearId, cancellationToken);
            if (!obligationCalculationsResponse.IsSuccessStatusCode || obligationCalculationsResponse.Result == null)
                return CannotLoadObligationCalculations(schemeYearId, obligationCalculationsResponse.Problem);
            var obligationCalculations = obligationCalculationsResponse.Result;

            var annualSales = await _annualBoilerSalesRepository.Get(s => s.OrganisationId == organisationId && s.SchemeYearId == schemeYearId);

            List<Entities.QuarterlyBoilerSales>? quarterlySales = null;

            // We only need quartly sales if there are no annual sales
            if(annualSales == null)
                quarterlySales = await _quarterlyBoilerSalesRepository.Get(s => s.OrganisationId == organisationId && s.SchemeYearId == schemeYearId);

            var summary = _boilerSalesCalculator.GenerateBoilerSalesSummary(organisationId, annualSales, quarterlySales, obligationCalculations);

            return summary;
        }
    }
}
