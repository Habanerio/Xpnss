namespace Habanerio.Xpnss.Shared.Types;

public static class OfxTransactionEnums
{
    /// <summary>
    /// 11.4.4.3 Transaction Types Used in `TRNTYPE`
    /// </summary>
    public enum OfxTransactionKeys
    {
        /// <summary>
        /// Generic credit
        /// </summary>
        CREDIT,

        /// <summary>
        /// Generic debit
        /// </summary>
        DEBIT,

        /// <summary>
        /// Interest earned or paid
        /// Note: Depends on signage of amount
        /// </summary>
        INT,

        /// <summary>
        /// Dividend
        /// </summary>
        DIV,

        /// <summary>
        /// FI fee
        /// </summary>
        FEE,

        /// <summary>
        /// Service charge
        /// </summary>
        SRVCHG,

        /// <summary>
        /// Deposit
        /// </summary>
        DEP,

        /// <summary>
        /// ATM debit or credit
        ///Note: Depends on signage of amount
        /// </summary>
        ATM,

        /// <summary>
        /// Point of sale debit or credit
        /// Note: Depends on signage of amount
        /// </summary>
        POS,

        /// <summary>
        /// Transfer
        /// </summary>
        XFER,

        /// <summary>
        /// Check
        /// </summary>
        CHECK,

        /// <summary>
        /// Electronic payment
        /// </summary>
        PAYMENT,

        /// <summary>
        /// Cash withdrawal
        /// </summary>
        CASH,

        /// <summary>
        /// Direct deposit
        /// </summary>
        DIRECTDEP,

        /// <summary>
        /// Merchant initiated debit
        /// </summary>
        DIRECTDEBIT,

        /// <summary>
        /// Repeating payment/standing order
        /// </summary>
        REPEATPMT,

        /// <summary>
        /// Only valid in `STMTTRNP`; indicates the amount is under a hold
        ///     Note: Depends on signage of amount and ofxAccount type
        /// </summary>
        HOLD,

        /// <summary>
        /// Other
        /// </summary>
        OTHER
    }
}