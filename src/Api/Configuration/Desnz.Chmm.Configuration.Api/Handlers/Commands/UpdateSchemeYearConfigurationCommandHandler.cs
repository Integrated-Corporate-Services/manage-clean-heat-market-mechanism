using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Api.Handlers.Commands;

public class UpdateSchemeYearConfigurationCommandHandler : BaseRequestHandler<UpdateSchemeYearConfigurationCommand, ActionResult>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISchemeYearRepository _schemeYearRepository;

    public UpdateSchemeYearConfigurationCommandHandler(
        ILogger<BaseRequestHandler<UpdateSchemeYearConfigurationCommand, ActionResult>> logger,
        IDateTimeProvider dateTimeProvider,
        ISchemeYearRepository schemeYearRepository) : base(logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _schemeYearRepository = schemeYearRepository;
    }

    public override async Task<ActionResult> Handle(UpdateSchemeYearConfigurationCommand command, CancellationToken cancellationToken)
    {
        var schemeYear = await _schemeYearRepository.GetSchemeYear(i => i.Id == command.SchemeYearId, true, true, true);
        if (schemeYear == null || schemeYear.ObligationCalculations == null)
            return CannotLoadSchemeYear(command.SchemeYearId);

        var now = _dateTimeProvider.UtcDateNow;

        if(schemeYear.StartDate <= now)
            return CannotEditSchemeYearConfiguration(command.SchemeYearId);

        schemeYear.ObligationCalculations.Update(
            command.TargetRate,
            command.PercentageCap,
            command.TargetMultiplier,
            command.CreditCarryOverPercentage,
            command.GasBoilerSalesThreshold,
            command.OilBoilerSalesThreshold
        );

        var values = schemeYear.CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(i => new
        {
            i.AlternativeSystemFuelTypeWeightingValue.Id,
            i.AlternativeSystemFuelTypeWeightingValue.Type
        }).Distinct();

        var fossilId = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel).Id;
        var renewableId = values.Single(i => i.Type == ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable).Id;

        var fossilValue = await _schemeYearRepository.GetAlternativeSystemFuelTypeWeightingValueById(fossilId, true);
        var renewableValue = await _schemeYearRepository.GetAlternativeSystemFuelTypeWeightingValueById(renewableId, true);

        fossilValue.Value = command.AlternativeFossilFuelSystemFuelTypeWeightingValue;
        renewableValue.Value = command.AlternativeRenewableSystemFuelTypeWeightingValue;

        await _schemeYearRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Responses.NoContent();
    }
}
