namespace Desnz.Chmm.CreditLedger.Constants;

public static class CreditLedgerConstants
{
    /// <summary>
    /// Types of transaction that can be created
    /// </summary>
    public static class TransactionType
    {
        /// <summary>
        /// Used for when credits are carried over to the next year
        /// </summary>
        public const string CarriedOverToNextYear = "Carried over to next year";
        
        /// <summary>
        /// Used for when credits are carried over from the previous year
        /// </summary>
        public const string CarriedOverFromPreviousYear = "Carried over from previous year";

        /// <summary>
        /// Used when transferring credits between organisations
        /// </summary>
        public const string Transfer = "Transfer";

        /// <summary>
        /// Used when an admin makes an adjustment to the number
        /// of credits an organisation has
        /// </summary>
        public const string AdminAdjustment = "Admin adjustment";

        /// <summary>
        /// Used to log how many credits were redeemed
        /// scheme year
        /// </summary>
        public const string Redeemed = "Redeemed";

        /// <summary>
        /// Used when admins are changing the ownership of a licence holder.
        /// </summary>
        //public const string OwnershipChangeAdjustment = "Ownership Change Adjustment";
    }

    /// <summary>
    /// Used to indicate the state of a transfer
    /// </summary>
    public static class CreditTransferStatus
    {
        /// <summary>
        /// Used when a transfer has been created
        /// </summary>
        public const string Created = "Created";
        /// <summary>
        /// Used to mark that a transfer has been accepted
        /// </summary>
        public const string Accepted = "Accepted";
    }
}
