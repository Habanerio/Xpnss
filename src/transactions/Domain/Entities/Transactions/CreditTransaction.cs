using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

public class CreditTransaction : TransactionBase
{
    protected CreditTransaction(
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
            true,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            extTransactionId)
    { }

    protected CreditTransaction(
            TransactionId id,
            UserId userId,
            AccountId accountId,
            TransactionEnums.TransactionKeys transactionType,
            PayerPayeeId payerPayeeId,
            Money amount,
            string description,
            DateTime transactionDate,
            IEnumerable<string>? tags,
            string externalTransactionId,
            DateTime dateCreated,
            DateTime? dateUpdated = null,
            DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            transactionType, true,
            payerPayeeId,
            amount,
            description,
            transactionDate,
            tags,
            externalTransactionId,
            dateCreated,
            dateUpdated,
            dateDeleted)
    { }
}