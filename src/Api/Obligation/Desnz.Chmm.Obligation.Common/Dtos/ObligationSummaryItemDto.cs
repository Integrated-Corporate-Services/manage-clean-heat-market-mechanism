
namespace Desnz.Chmm.Obligation.Common.Dtos
{
    /// <summary>
    /// Details of a single item for the Obligation Summary Dto
    /// </summary>
    public class ObligationSummaryItemDto
    {
        public ObligationSummaryItemDto(DateTime dateOfTransaction, decimal value)
        {
            DateOfTransaction = dateOfTransaction;
            Value = Decimal.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));
        }

        public DateTime DateOfTransaction { get; }
        public int Value { get; }
    }
}
