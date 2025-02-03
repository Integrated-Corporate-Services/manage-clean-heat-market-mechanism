using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Annual boiler sales changes
/// </summary>
public class AnnualBoilerSalesChange : Entity
{
    #region Constructors

    protected AnnualBoilerSalesChange() : base() { }

    public AnnualBoilerSalesChange(AnnualBoilerSales annualBoilerSales, int? oldGas, int? oldOil, string? oldStatus, int newGas, int newOil, string newStatus, string note, string? createdBy = null) : base(createdBy)
    {
        AnnualBoilerSales = annualBoilerSales ?? throw new ArgumentNullException(nameof(annualBoilerSales));
        AnnualBoilerSalesId = annualBoilerSales.Id;
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

    public Guid AnnualBoilerSalesId { get; private set; }

    /// <summary>
    /// Annual boiler sales change is for
    /// </summary>
    public AnnualBoilerSales AnnualBoilerSales { get; private set; }

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
