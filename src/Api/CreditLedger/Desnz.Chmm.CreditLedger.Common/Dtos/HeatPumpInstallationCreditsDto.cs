namespace Desnz.Chmm.CreditLedger.Common.Dtos;

/// <summary>
/// Defines the heat pump installation
/// total credits for all licence holders.
/// </summary>
public class HeatPumpInstallationCreditsDto
{
    #region Constructors
    public HeatPumpInstallationCreditsDto(int heatPumpInstallationId, decimal value)
    {
        HeatPumpInstallationId = heatPumpInstallationId;
        Value = value;
    }

    #endregion

    #region Properties
    /// <summary>
    /// A reference to the actual heat pump installation record from MCS
    /// </summary>
    public int HeatPumpInstallationId { get; private set; }

    /// <summary>
    /// The value of the credit
    /// </summary>
    public decimal Value { get; private set; }

    #endregion
}
