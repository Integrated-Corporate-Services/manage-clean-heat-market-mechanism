namespace Desnz.Chmm.CreditLedger.Common.Dtos;

/// <summary>
/// Defines the current balance for an organisation
/// </summary>
public class CreditBalanceDto
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="creditBalance">The credit balance</param>
    public CreditBalanceDto(decimal creditBalance)
    {
        CreditBalance = creditBalance;
    }

    /// <summary>
    /// The current balance of credits for an organisation
    /// </summary>
    public decimal CreditBalance { get; }
}
