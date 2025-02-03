using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Commands;

public class UpdateSchemeYearConfigurationCommand : IRequest<ActionResult>
{
    public Guid SchemeYearId { get; init; }
    public decimal TargetRate { get; init; }
    public decimal PercentageCap { get; init; }
    public decimal TargetMultiplier { get; init; }
    public decimal CreditCarryOverPercentage { get; init; }
    public int GasBoilerSalesThreshold { get; init; }
    public int OilBoilerSalesThreshold { get; init; }
    public decimal AlternativeRenewableSystemFuelTypeWeightingValue { get; init; }
    public decimal AlternativeFossilFuelSystemFuelTypeWeightingValue { get; init; }
}
