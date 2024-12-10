using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class DebitTransaction : TransactionBase
{
    protected DebitTransaction(
            UserId userId,
            AccountId accountId,
            TransactionEnums.TransactionKeys transactionType,
            PayerPayeeId payerPayeeId,
            Money amount,
            string description,
            DateTime transactionDate,
            IEnumerable<string>? tags = null,
            string extTransactionId = "") :
        base(
            userId,
            accountId,
            transactionType,
            false,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId)
    { }

    protected DebitTransaction(
            TransactionId id,
            UserId userId,
            AccountId accountId,
            TransactionEnums.TransactionKeys transactionType,
            PayerPayeeId payerPayeeId,
            Money amount,
            string description,
            DateTime transactionDate,
            IEnumerable<string>? tags,
            string extTransactionId,
            DateTime dateCreated,
            DateTime? dateUpdated = null,
            DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            transactionType,
            false,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }
}