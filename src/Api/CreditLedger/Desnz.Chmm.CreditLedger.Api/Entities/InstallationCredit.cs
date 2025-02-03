using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.CreditLedger.Api.Entities;

/// <summary>
/// Defines a credit generated for a licence holder's 
/// heat pump installation. Can be either 1 or 0.5
/// </summary>
public class InstallationCredit : Entity
{
    #region Constructors

    /// <summary>
    /// Default constructor for EF
    /// </summary>
    protected InstallationCredit() : base() { }

    public InstallationCredit(Guid licenceHolderId, int heatPumpInstallationId, Guid schemeYearId, DateOnly dateCreditGenerated, decimal value, bool isHybrid)
    {
        LicenceHolderId = licenceHolderId;
        HeatPumpInstallationId = heatPumpInstallationId;
        SchemeYearId = schemeYearId;
        DateCreditGenerated = dateCreditGenerated;
        Value = value;
        IsHybrid = isHybrid;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The licence holder for the product that was installed
    /// </summary>
    public Guid LicenceHolderId { get; private set; }

    /// <summary>
    /// A reference to the actual heat pump installation record from MCS
    /// </summary>
    public int HeatPumpInstallationId { get; private set; }

    /// <summary>
    /// The scheme year this credit is associated with
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// Date the credit was created
    /// </summary>
    public DateOnly DateCreditGenerated { get; private set; }

    /// <summary>
    /// The value of the credit
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Does credit from hybrid heat pump
    /// </summary>
    public bool IsHybrid { get; private set; }

    #endregion

    #region Overrides

    public override int GetHashCode()
    {
        return HashCode.Combine(LicenceHolderId, HeatPumpInstallationId, SchemeYearId, IsHybrid);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as InstallationCredit);
    }

    public bool Equals(InstallationCredit other)
    {
        return other is not null &&
               LicenceHolderId == other.LicenceHolderId &&
               HeatPumpInstallationId == other.HeatPumpInstallationId &&
               SchemeYearId == other.SchemeYearId &&
               IsHybrid == other.IsHybrid;
    }
    #endregion
}
