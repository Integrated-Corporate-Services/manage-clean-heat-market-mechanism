using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;

/// <summary>
/// Quarterly boiler sales file
/// </summary>
public class QuarterlyBoilerSalesFileDto : Entity
{
    #region Properties

    public Guid QuarterlyBoilerSalesId { get; set; }

    /// <summary>
    /// File key (AWS reference)
    /// </summary>
    public string FileKey { get; set; }

    /// <summary>
    /// Name of file
    /// </summary>
    public string FileName { get; set; }

    #endregion
}
