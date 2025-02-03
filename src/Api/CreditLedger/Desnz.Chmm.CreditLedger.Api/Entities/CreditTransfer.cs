using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.CreditLedger.Constants;

namespace Desnz.Chmm.CreditLedger.Api.Entities;

/// <summary>
/// Stores a record of a transfer before it is accepted and turned
/// into a transaction.
/// </summary>
public class CreditTransfer : Entity
{
    #region Constructors
    /// <summary>
    /// Default constructor for EF
    /// </summary>
    protected CreditTransfer() :base() { }

    /// <summary>
    /// Register a transfer and set it up ready to go
    /// </summary>
    /// <param name="organisationId">The source organisation</param>
    /// <param name="destinationOrganisationId">The destination organisation</param>
    /// <param name="schemeYearId">The year of the transfer</param>
    /// <param name="createdBy">The user creating the transaction</param>
    /// <param name="value">The value of the transfer</param>
    public CreditTransfer(Guid organisationId, Guid destinationOrganisationId, Guid schemeYearId, Guid createdBy, decimal value) : base(createdBy.ToString())
    {
        SourceOrganisationId = organisationId;
        DestinationOrganisationId = destinationOrganisationId;
        SchemeYearId = schemeYearId;
        Value = value;
        Status = CreditLedgerConstants.CreditTransferStatus.Created;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The organisation credits will be taken from
    /// </summary>
    public Guid SourceOrganisationId { get; private set; }

    /// <summary>
    /// The organisation the credits will be given to
    /// </summary>
    public Guid DestinationOrganisationId { get; private set; }

    /// <summary>
    /// The scheme year the transaction happens in
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// The number of credits that are to be transferred
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Transaction <see cref="CreditLedgerConstants.CreditTransferStatus">status</see>
    /// Has it been created or accepted?
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// The Transaction Id that is created by the accepted transfer
    /// </summary>
    public Guid? TransactionId { get; private set; }

    /// <summary>
    /// The transaction that is created by the accepted transfer
    /// </summary>
    public Transaction? Transaction { get; private set; }
    #endregion

    #region Benaviour
    /// <summary>
    /// Actually accept the transfer and create the credit ledger entry.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void AcceptTransfer()
    {
        Transaction = new Transaction(SchemeYearId, SourceOrganisationId, DestinationOrganisationId, Value, new Guid(CreatedBy), CreationDate);        
    }
    #endregion
}
