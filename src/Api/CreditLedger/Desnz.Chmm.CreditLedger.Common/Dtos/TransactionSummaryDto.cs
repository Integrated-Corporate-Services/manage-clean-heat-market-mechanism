namespace Desnz.Chmm.CreditLedger.Common.Dtos;

/// <summary>
/// Transaction
/// </summary>
public class TransactionSummaryDto
{
    public TransactionSummaryDto(
        decimal creditsTransferredIn, 
        decimal creditsTransferredOut, 
        IList<TransactionSummaryAdminAdjustmentDto> creditsAmendedByAdministrator, 
        decimal creditsCarriedOverFromPreviousSchemeYear, 
        decimal creditsCarriedOverToNextSchemeYear, 
        decimal? creditsRedeemed)
    {
        CreditsTransferredIn = creditsTransferredIn;
        CreditsTransferredOut = creditsTransferredOut;
        CreditsAmendedByAdministrator = creditsAmendedByAdministrator;
        CreditsCarriedOverFromPreviousSchemeYear = creditsCarriedOverFromPreviousSchemeYear;
        CreditsCarriedOverToNextSchemeYear = creditsCarriedOverToNextSchemeYear;
        CreditsRedeemed = creditsRedeemed;
    }

    public decimal CreditsTransferredIn { get; }
    public decimal CreditsTransferredOut { get; }
    public IList<TransactionSummaryAdminAdjustmentDto> CreditsAmendedByAdministrator { get; }
    public decimal CreditsCarriedOverFromPreviousSchemeYear { get; }
    public decimal CreditsCarriedOverToNextSchemeYear { get; }
    public decimal? CreditsRedeemed { get; }
}

/// <summary>
/// Admin adjustment DTO
/// </summary>
public class TransactionSummaryAdminAdjustmentDto
{
    public TransactionSummaryAdminAdjustmentDto(Guid initiatedBy, DateTime dateOfTransaction, decimal credits)
    {
        InitiatedBy = initiatedBy;
        DateOfTransaction = dateOfTransaction;
        Credits = credits;
    }

    public Guid InitiatedBy { get; }
    public DateTime DateOfTransaction { get; }
    public decimal Credits { get; }
}