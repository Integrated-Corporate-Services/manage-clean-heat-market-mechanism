using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Api.Handlers.Queries
{
    public class GetSchemeYearSummaryConfigurationQueryHandler : BaseRequestHandler<GetSchemeYearSummaryConfigurationQuery, ActionResult<SchemeYearSummaryConfigurationDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetSchemeYearSummaryConfigurationQueryHandler(
            ILogger<BaseRequestHandler<GetSchemeYearSummaryConfigurationQuery, ActionResult<SchemeYearSummaryConfigurationDto>>> logger,
            ISchemeYearRepository schemeYearRepository,
            IDateTimeProvider dateTimeProvider) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public override async Task<ActionResult<SchemeYearSummaryConfigurationDto>> Handle(GetSchemeYearSummaryConfigurationQuery request, CancellationToken cancellationToken)
        {
            var schemeYear = await _schemeYearRepository.GetSchemeYearById(request.SchemeYearId, includeObligationCalculations: true, includeWeightings: true);

            if (schemeYear == null)
                return CannotLoadSchemeYear(request.SchemeYearId);
            var now = _dateTimeProvider.UtcDateNow;

            var isAfterEndOfSchemeYear = now > schemeYear.EndDate;
            var isAfterSurrenderDay = now > schemeYear.SurrenderDayDate;

            var isWithinAmendObligationsWindow = now >= schemeYear.StartDate && now < schemeYear.SurrenderDayDate;
            var isWithinAmendCreditsWindow = now >= schemeYear.StartDate && now < schemeYear.SurrenderDayDate;
            var isWithinCreditTranferWindow = now >= schemeYear.TradingWindowStartDate && now <= schemeYear.TradingWindowEndDate;

            // Previous scheme year date
            bool isAfterPreviousSchemeYearSurrenderDate;
            if (schemeYear.PreviousSchemeYearId == null)
                isAfterPreviousSchemeYearSurrenderDate = false;
            else
            {
                var previousShemeYear = await _schemeYearRepository.GetSchemeYearById(schemeYear.PreviousSchemeYearId.Value);
                if (previousShemeYear == null)
                    return CannotLoadSchemeYearByPreviousId(schemeYear.PreviousSchemeYearId.Value);

                isAfterPreviousSchemeYearSurrenderDate = now > previousShemeYear.SurrenderDayDate;
            }

            var values = schemeYear.CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(i => new
            {
                i.AlternativeSystemFuelTypeWeightingValue.Type,
                i.AlternativeSystemFuelTypeWeightingValue.Value
            }).Distinct();

            var fossilValue = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel).Value;
            var renewableValue = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable).Value;

            return new SchemeYearSummaryConfigurationDto(
                schemeYear.Name, 
                schemeYear.ObligationCalculations.TargetRate,
                isAfterEndOfSchemeYear,
                isAfterSurrenderDay,
                isAfterPreviousSchemeYearSurrenderDate,
                isWithinAmendObligationsWindow,
                isWithinAmendCreditsWindow,
                isWithinCreditTranferWindow,
                renewableValue,
                fossilValue);
        }
    }
}
