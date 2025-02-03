using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.CreditLedger.Api.Entities;

/// <summary>
/// Defines the organisation and value of the transaction
/// </summary>
public class TransactionEntry : Entity
{
    public TransactionEntry(Guid organisationId, decimal value)
    {
        OrganisationId = organisationId;
        Value = value;
    }
    #region Constructors
    /// <summary>
    /// Default constructor for EF
    /// </summary>
    protected TransactionEntry() : base() { }
    #endregion

    #region Properties
    /// <summary>
    /// The Id of the transaction this entry belongs to
    /// </summary>
    public Guid TransactionId { get; private set; }

    /// <summary>
    /// The Transaction this entry belongs to
    /// </summary>
    public Transaction Transaction { get; private set; }
    
    /// <summary>
    /// The Id of the organisation this transaction belongs to
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// The value of the transaction
    /// </summary>
    public decimal Value { get; set; }
    #endregion
}