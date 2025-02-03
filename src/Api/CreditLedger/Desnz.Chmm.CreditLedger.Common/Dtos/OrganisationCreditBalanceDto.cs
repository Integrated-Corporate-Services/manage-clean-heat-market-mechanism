namespace Desnz.Chmm.CreditLedger.Common.Dtos;

/// <summary>
/// Defines the current balance for an organisation
/// </summary>
public class OrganisationCreditBalanceDto
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="creditBalance">The credit balance</param>
    public OrganisationCreditBalanceDto(Guid organisationId, decimal creditBalance)
    {
        OrganisationId = organisationId;
        CreditBalance = creditBalance;
    }

    /// <summary>
    /// The Id of the organisation
    /// </summary>
    public Guid OrganisationId { get; }

    /// <summary>
    /// The current balance of credits for an organisation
    /// </summary>
    public decimal CreditBalance { get; }
}