using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Common.Dtos;
public class SalesNumbersDto : Entity
{
    /// <summary>
    /// Gas boiler sales for scheme year quarter
    /// </summary>
    public int Gas { get; set; }

    /// <summary>
    /// Oil boiler sales for scheme year quarter
    /// </summary>
    public int Oil { get; set; }
}
