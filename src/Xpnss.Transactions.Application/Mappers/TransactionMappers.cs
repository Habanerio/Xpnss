using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

namespace Habanerio.Xpnss.Transactions.Application.Mappers;

internal static partial class ApplicationMapper
{
    public static IEnumerable<TransactionDto> Map(IEnumerable<TransactionBase> entities)
    {
        var results = new List<TransactionDto>();

        foreach (var entity in entities)
        {
            TransactionDto? dto = Map(entity);

            if (dto is not null)
                results.Add(dto);
        }

        return results;
    }

    public static TransactionDto? Map(TransactionBase? entity)
    {
        if (entity is null)
            return default;

        if (entity is DepositTransaction depositEntity)
            return Map(depositEntity);

        if (entity is PurchaseTransaction purchaseEntity)
            return Map(purchaseEntity);

        throw new InvalidOperationException("Invalid transaction type");
    }

    public static DepositTransactionDto? Map(DepositTransaction? entity)
    {
        if (entity is null)
            return default;

        if (!entity.TransactionType.Equals(TransactionEnums.TransactionKeys.DEPOSIT))
            throw new InvalidOperationException("Invalid transaction type");

        return new DepositTransactionDto()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountId = entity.AccountId,
            ExtTransactionId = entity.ExtTransactionId,
            TotalAmount = entity.TotalAmount,
            Description = entity.Description,
            PayerPayeeId = entity.PayerPayeeId,
            Tags = entity.Tags.ToList(),
            TransactionDate = entity.TransactionDate,
        };
    }

    /// <summary>
    /// Converts a PurchaseTransaction to a PurchaseTransactionDto
    /// </summary>
    /// <param name="entity">The Purchase Transaction Entity</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TransactionDto? Map(PurchaseTransaction? entity)
    {
        if (entity is null)
            return default;

        if (!entity.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE))
            throw new InvalidOperationException("Invalid transaction type");

        return new PurchaseTransactionDto()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountId = entity.AccountId,
            Description = entity.Description,
            Items = Map(entity.Items).ToList(),
            PayerPayeeId = entity.PayerPayeeId.Value,
            Tags = entity.Tags.ToList(),
            TotalAmount = entity.TotalAmount,
            TotalPaid = entity.TotalPaid,
            TransactionDate = entity.TransactionDate,
            PaidDate = entity.DatePaid,
        };
    }

    public static IEnumerable<TransactionItemDto> Map(IEnumerable<TransactionItem> items)
    {
        var results = new List<TransactionItemDto>();

        foreach (var item in items)
        {
            var dto = Map(item);

            if (dto is not null)
                results.Add(dto);
        }

        return results;
    }

    public static TransactionItemDto? Map(TransactionItem? item)
    {
        if (item is null)
            return default;

        return new TransactionItemDto
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            Description = item.Description,
            Amount = item.Amount,
        };
    }
}