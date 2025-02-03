using Desnz.Chmm.Common.ValueObjects;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Queries
{
    /// <summary>
    /// Query the organisation's credit ledger summary for the given year
    /// </summary>
    public class GetInstallationCreditsQuery : InstallationPeriod<ActionResult<List<HeatPumpInstallationCreditsDto>>>
    {
        public GetInstallationCreditsQuery(DateOnly startDate, DateOnly endDate) : base(startDate.ToDateTime(new TimeOnly()), endDate.ToDateTime(new TimeOnly()))
        {
        }
    }
}
