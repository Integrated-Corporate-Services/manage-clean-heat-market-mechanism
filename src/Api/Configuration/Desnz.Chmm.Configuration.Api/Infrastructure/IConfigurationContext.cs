using Desnz.Chmm.Common;
using Desnz.Chmm.Configuration.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Configuration.Api.Infrastructure;

public interface IConfigurationContext : IUnitOfWork
{
    DbSet<SchemeYear> SchemeYears { get; set; }
    DbSet<SchemeYearQuarter> SchemeYearQuarters { get; set; }
    DbSet<CreditWeighting> CreditWeightings { get; set; }
    DbSet<ObligationCalculations> ObligationCalculations { get; set; }
    DbSet<AlternativeSystemFuelTypeWeighting> AlternativeSystemFuelTypeWeightings { get; set; }
    DbSet<AlternativeSystemFuelTypeWeightingValue> AlternativeSystemFuelTypeWeightingValues { get; set; }
    DbSet<HeatPumpTechnologyTypeWeighting> HeatPumpTechnologyTypeWeightings { get; set; }
}
