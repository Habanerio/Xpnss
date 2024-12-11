using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

//TODO: This should probably derive from a 'TransferTransaction' class
// The opposite of this transaction should be a deposit
public class WithdrawalTransaction : DebitTransaction
{
    /// <summary>
    /// The Id of the ofxAccount that the withdrawal was made to (eg: from Checking to Cash/Wallet).
    /// </summary>
    public AccountId? WithdrewToId { get; }

    protected WithdrawalTransaction(
        UserId userId,
        AccountId accountId,
        AccountId? withdrewToId,
        string description,
        string extTransactionId,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate) :
        base(
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.WITHDRAWAL)
    {
        WithdrewToId = withdrewToId;
    }

    protected WithdrawalTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        AccountId? withdrewToAccountId,
        string description,
        string extTransactionId,
        TransactionItem item,
        PayerPayeeId payerPayeeId,
        IEnumerable<string>? tags,
        DateTime transactionDate,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            description,
            extTransactionId,
            item,
            payerPayeeId,
            tags,
            transactionDate,
            TransactionEnums.TransactionKeys.WITHDRAWAL,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        WithdrewToId = withdrewToAccountId;
    }
}