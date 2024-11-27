using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities;

public class DepositTransaction : TransactionBase
{
    // Not sure if I want this nullable, or non-nullable with a "" value?
    // Same with CategoryId
    public PayerPayeeId PayerPayeeId { get; }

    protected DepositTransaction(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags) :
        base(userId, accountId, TransactionTypes.Keys.DEPOSIT, amount, description, transactionDate, tags)
    {
        PayerPayeeId = payerPayeeId;
    }

    protected DepositTransaction(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) :
        base(
            id,
            userId,
            accountId,
            TransactionTypes.Keys.DEPOSIT,
            amount,
            description,
            transactionDate,
            tags,
            dateCreated,
            dateUpdated,
            dateDeleted)
    {
        PayerPayeeId = payerPayeeId;
    }

    public static DepositTransaction Load(
        TransactionId id,
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null)
    {
        return new DepositTransaction(
            id,
            userId,
            accountId,
            amount,
            description,
            payerPayeeId,
            transactionDate,
            tags,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static DepositTransaction New(
        UserId userId,
        AccountId accountId,
        Money amount,
        string description,
        PayerPayeeId payerPayeeId,
        DateTime transactionDate,
        IEnumerable<string>? tags)
    {
        return new DepositTransaction(
            userId,
            accountId,
            amount,
            description,
            payerPayeeId,
            transactionDate,
            tags);
    }
}