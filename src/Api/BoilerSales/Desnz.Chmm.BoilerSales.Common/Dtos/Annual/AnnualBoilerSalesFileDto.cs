using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Common.Dtos.Annual;

/// <summary>
/// Annual boiler sales file
/// </summary>
public class AnnualBoilerSalesFileDto : Entity
{
    #region Properties

    public Guid AnnualBoilerSalesId { get; set; }

    /// <summary>
    /// File key (AWS reference)
    /// </summary>
    public string FileKey { get; set; }

    /// <summary>
    /// Name of file
    /// </summary>
    public string FileName { get; set; }

    public string Type { get; set; }

    #endregion
}
