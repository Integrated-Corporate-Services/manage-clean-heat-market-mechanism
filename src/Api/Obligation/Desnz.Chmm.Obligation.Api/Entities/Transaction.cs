using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Common.Dtos;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.Api.Entities;

/// <summary>
/// Stores a record of a transaction before it is accepted and turned
/// into a transaction.
/// </summary>
public class Transaction : Entity
{
    #region Constructors
    /// <summary>
    /// Default constructor for EF
    /// </summary>
    protected Transaction() : base() { }

    public Transaction(Guid userId, string transactionType, Guid organisationId, Guid schemeYearId, int obligation, DateTime transactionDate, Guid? schemeYearQuarterId = null) : base() 
    {
        if (!TransactionTypes.Contains(transactionType))
        {
            throw new ArgumentException($"Invalid {nameof(transactionType)}");
        };

        UserId = userId;
        TransactionType = transactionType;
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        Obligation = obligation;
        IsExcluded = false;
        SchemeYearQuarterId = schemeYearQuarterId;
        DateOfTransaction = transactionDate;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The user who initialised the transaction
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The <see cref="TransactionConstants.TransactionType">type</see> of the transaction
    /// </summary>
    public string TransactionType { get; private set; }

    /// <summary>
    /// Organisation Id
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// The date the transaction was initialised
    /// </summary>
    public DateTime DateOfTransaction { get; private set; }

    /// <summary>
    /// The scheme year the transaction happens in
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// Scheme year quarter boiler sales are reported for
    /// </summary>
    public Guid? SchemeYearQuarterId { get; private set; }

    /// <summary>
    /// When someone submits Annual boiler data, find the quarter transactions for that scheme year and mark them as Excluded
    /// </summary>
    public bool IsExcluded { get; private set; }
    /// <summary>
    /// The number of obligations
    /// </summary>
    public int Obligation { get; private set; }
    #endregion

    #region Behaviours
    /// <summary>
    /// Excludes the transaction from calculations
    /// </summary>
    public void Exclude()
    {
        IsExcluded = true;
    }

    /// <summary>
    /// Convert an item into an obligation summary item
    /// </summary>
    /// <returns></returns>
    internal ObligationSummaryItemDto ToObligationSummaryItemDto()
    {
        return new ObligationSummaryItemDto(DateOfTransaction, Obligation);
    }
    #endregion
}
