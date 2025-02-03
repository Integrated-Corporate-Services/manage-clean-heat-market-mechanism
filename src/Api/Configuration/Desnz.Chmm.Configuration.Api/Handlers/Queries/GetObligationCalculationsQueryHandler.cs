using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetObligationCalculationsQueryHandler : BaseRequestHandler<GetObligationCalculationsQuery, ActionResult<ObligationCalculationsDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        public GetObligationCalculationsQueryHandler(
            ILogger<BaseRequestHandler<GetObligationCalculationsQuery, ActionResult<ObligationCalculationsDto>>> logger,
            ISchemeYearRepository schemeYearRepository,
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<ObligationCalculationsDto>> Handle(GetObligationCalculationsQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = _schemeYearRepository.GetObligationCalculations(i => i.SchemeYearId == request.SchemeYearId);
            if (schemeYearData == null)
                return CannotLoadObligationCalculations(request.SchemeYearId);

            var schemeYear = await _schemeYearRepository.GetSchemeYearById(request.SchemeYearId, true, false, false);
            if (schemeYear == null)
                return CannotLoadSchemeYear(request.SchemeYearId);

            var values = schemeYear.CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(i => new
            {
                i.AlternativeSystemFuelTypeWeightingValue.Value,
                i.AlternativeSystemFuelTypeWeightingValue.Type
            }).Distinct();

            var fossilValue = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel).Value;
            var renewableValue = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable).Value;

            var mappedData = new ObligationCalculationsDto { 
                AlternativeFossilFuelSystemFuelTypeWeightingValue = fossilValue,
                AlternativeRenewableSystemFuelTypeWeightingValue = renewableValue,
                CreditCarryOverPercentage = schemeYearData.CreditCarryOverPercentage,
                GasBoilerSalesThreshold  = schemeYearData.GasBoilerSalesThreshold,
                GasCreditsCap = schemeYearData.GasCreditsCap,
                OilBoilerSalesThreshold = schemeYearData.OilBoilerSalesThreshold,
                OilCreditsCap = schemeYearData.OilCreditsCap,
                PercentageCap = schemeYearData.PercentageCap,
                TargetMultiplier = schemeYearData.TargetMultiplier,
                TargetRate = schemeYearData.TargetRate
            };

            return mappedData;
        }
    }
}

