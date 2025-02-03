namespace Desnz.Chmm.CreditLedger.Common.Dtos
{
    public class CreditLedgerTranferBaseDto
    {
        public CreditLedgerTranferBaseDto(DateTime transferDateTime, string organisationName, decimal value)
        {
            TransferDateTime = transferDateTime;
            OrganisationName = organisationName;
            Value = value;
        }

        public DateTime TransferDateTime { get; private set; }
        public string OrganisationName { get; private set; }
        public decimal Value { get; private set; }
    }

    public class CreditLedgerTransferInDto : CreditLedgerTranferBaseDto
    {
        public CreditLedgerTransferInDto(DateTime transferDateTime, string sourceOrganisationName, decimal value)
            : base(transferDateTime, sourceOrganisationName, value)
        { }
    }

    public class CreditLedgerTransferOutDto : CreditLedgerTranferBaseDto
    {
        public CreditLedgerTransferOutDto(DateTime transferDateTime, string destinationOrganisationName, decimal value, string transferredBy)
            : base(transferDateTime, destinationOrganisationName, value)
        {
            TransferredBy = transferredBy;
        }
        public string TransferredBy { get; private set; }
    }

    public class CreditLedgerTransfersDto
    {
        public CreditLedgerTransfersDto(IList<CreditLedgerTransferInDto> transfersIn, IList<CreditLedgerTransferOutDto> transfersOut)
        {
            TransfersIn = transfersIn;
            TransfersOut = transfersOut;
        }

        public IList<CreditLedgerTransferInDto> TransfersIn { get; private set; }
        public IList<CreditLedgerTransferOutDto> TransfersOut { get; private set; }
    }
}
