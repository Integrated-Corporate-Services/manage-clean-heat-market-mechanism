using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.CreditLedger.Constants;

namespace Desnz.Chmm.CreditLedger.Api.Entities;

/// <summary>
/// A transaction record
/// </summary>
public class Transaction : Entity
{
    #region Constructors
    /// <summary>
    /// Default constructor for EF
    /// </summary>
    protected Transaction() : base() { }

    /// <summary>
    /// Constructs a single transaction entity
    /// I expect this to be replaced as the features expand
    /// </summary>
    /// <param name="schemeYearId">The scheme year of the tranation</param>
    /// <param name="organisationId">The organisation concerned</param>
    /// <param name="value">The number of credits being moved</param>
    /// <param name="initiatedBy">The id of the user initiating the transfer</param>
    /// <param name="transactionType"><see cref="CreditLedgerConstants.TransactionType"/> the type of the transaction</param>
    /// <param name="dateOfTransaction">The date of the transaction</param>
    public Transaction(Guid schemeYearId, Guid organisationId, decimal value, Guid initiatedBy, string transactionType, DateTime dateOfTransaction) : base()
    {
        SchemeYearId = schemeYearId;
        InitiatedBy = initiatedBy;
        TransactionType = transactionType;
        DateOfTransaction = dateOfTransaction;
        _entries = new List<TransactionEntry>
        {
            new TransactionEntry(organisationId, value)
        };
    }

    /// <summary>
    /// Constructs a single transaction entity
    /// I expect this to be replaced as the features expand
    /// </summary>
    /// <param name="schemeYearId">The scheme year of the tranation</param>
    /// <param name="fromOrganisationId">The organisation credits are coming from</param>
    /// <param name="toOrganisationId">The organisation credits are going to</param>
    /// <param name="value">The value of the transaction</param>
    /// <param name="initiatedBy">The user who created the transfer</param>
    /// <param name="dateOfTransaction">The date of the transfer</param>
    public Transaction(Guid schemeYearId, Guid fromOrganisationId, Guid toOrganisationId, decimal value, Guid initiatedBy, DateTime dateOfTransaction)
    {
        SchemeYearId = schemeYearId;
        InitiatedBy = initiatedBy;
        DateOfTransaction = dateOfTransaction;
        TransactionType = CreditLedgerConstants.TransactionType.Transfer;
        _entries = new List<TransactionEntry>
        {
            new TransactionEntry(fromOrganisationId, -value),
            new TransactionEntry(toOrganisationId, value)
        };
    }
    #endregion

    #region Private fields
    private List<TransactionEntry> _entries;
    #endregion

    #region Properties
    /// <summary>
    /// The user who created the transaction
    /// </summary>
    public Guid InitiatedBy { get; private set; }

    /// <summary>
    /// Date the transaction was initiated
    /// </summary>
    public DateTime DateOfTransaction { get; private set; }

    /// <summary>
    /// The <see cref="CreditLedgerConstants.TransactionType">type</see> of the transaction
    /// </summary>
    public string TransactionType { get; private set; }

    /// <summary>
    /// The scheme year the transaction is associated with
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// A link to the Credit Transfer that created this transaction
    /// If one exists
    /// </summary>
    public CreditTransfer? CreditTransfer { get; private set; }

    /// <summary>
    /// A record of all transaction entires associated with the transaction
    /// </summary>
    public IReadOnlyCollection<TransactionEntry> Entries => _entries;

    #endregion
}
