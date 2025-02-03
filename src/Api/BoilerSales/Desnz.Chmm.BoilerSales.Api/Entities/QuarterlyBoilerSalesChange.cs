using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Quarterly boiler sales changes
/// </summary>
public class QuarterlyBoilerSalesChange : Entity
{
    #region Constructors

    protected QuarterlyBoilerSalesChange() : base() { }

    public QuarterlyBoilerSalesChange(QuarterlyBoilerSales quarterlyBoilerSales, int? oldGas, int? oldOil, string? oldStatus, int newGas, int newOil, string newStatus, string note, string? createdBy = null) : base(createdBy)
    {
        QuarterlyBoilerSales = quarterlyBoilerSales ?? throw new ArgumentNullException(nameof(quarterlyBoilerSales));
        QuarterlyBoilerSalesId = quarterlyBoilerSales.Id;
        OldGas = oldGas;
        OldOil = oldOil;
        OldStatus = oldStatus;
        NewGas = newGas;
        NewOil = newOil;
        NewStatus = newStatus;
        Note = note;
    }

    #endregion

    #region Properties

    public Guid QuarterlyBoilerSalesId { get; private set; }

    /// <summary>
    /// Quarterly boiler sales change is for
    /// </summary>
    public QuarterlyBoilerSales QuarterlyBoilerSales { get; private set; }

    #region Old

    /// <summary>
    /// Previous value for gas boiler sales
    /// </summary>
    public int? OldGas { get; private set; }

    /// <summary>
    /// Previous value for oil boiler sales
    /// </summary>
    public int? OldOil { get; private set; }

    /// <summary>
    /// Previous value for admin review status
    /// </summary>
    public string? OldStatus { get; private set; }

    #endregion

    #region New

    /// <summary>
    /// New value for gas boiler sales
    /// </summary>
    public int NewGas { get; private set; }

    /// <summary>
    /// New value for oil boiler sales
    /// </summary>
    public int NewOil { get; private set; }

    /// <summary>
    /// New value for admin review status
    /// </summary>
    public string NewStatus { get; private set; }

    #endregion

    /// <summary>
    /// Note associated with this change
    /// </summary>
    public string Note { get; private set; }

    #endregion
}
