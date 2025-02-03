using Desnz.Chmm.Common.ValueObjects;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Queries;

public class GetManufacturerInstallationsQuery : InstallationPeriod<ActionResult<List<CreditCalculationDto>>>
{
    /// <summary>
    /// Ges manufacurer installations for Id, StartDate and EndDate
    /// </summary>
    /// <param name="manufacturerId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    public GetManufacturerInstallationsQuery(int manufacturerId, DateTime startDate, DateTime endDate) : base(startDate, endDate)
    {
        ManufacturerId = manufacturerId;
    }

    public int ManufacturerId { get; }
}
