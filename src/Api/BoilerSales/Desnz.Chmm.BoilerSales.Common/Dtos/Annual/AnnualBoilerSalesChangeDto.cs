using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Common.Dtos.Annual;

/// <summary>
/// Annual boiler sales changes
/// </summary>
public class AnnualBoilerSalesChangeDto : Entity
{
    #region Properties

    public Guid AnnualBoilerSalesId { get; set; }

    #region Old

    /// <summary>
    /// Previous value for gas boiler sales
    /// </summary>
    public int? OldGas { get; set; }

    /// <summary>
    /// Previous value for oil boiler sales
    /// </summary>
    public int? OldOil { get; set; }

    /// <summary>
    /// Previous value for admin review status
    /// </summary>
    public string? OldStatus { get; set; }

    #endregion

    #region New

    /// <summary>
    /// New value for gas boiler sales
    /// </summary>
    public int NewGas { get; set; }

    /// <summary>
    /// New value for oil boiler sales
    /// </summary>
    public int NewOil { get; set; }

    /// <summary>
    /// New value for admin review status
    /// </summary>
    public string NewStatus { get; set; }

    #endregion

    /// <summary>
    /// Note associated with this change
    /// </summary>
    public string Note { get; set; }

    #endregion
}
