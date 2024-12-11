using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;

namespace Habanerio.Xpnss.Transactions.Application.Mappers;

internal static class ApplicationMapper
{
    /*
    private static readonly Dictionary<TransactionEnums.TransactionKeys, Type> TypeDtoMapper =
        new Dictionary<TransactionEnums.TransactionKeys, Type>
        {
            { TransactionEnums.TransactionKeys.DEPOSIT, typeof(DepositTransactionDto) },
            { TransactionEnums.TransactionKeys.PURCHASE, typeof(PurchaseTransactionDto) },
            { TransactionEnums.TransactionKeys.WITHDRAWAL, typeof(WithdrawalTransactionDto) }
        };
    */

    public static IEnumerable<TransactionDto> Map(IEnumerable<Transaction> entities)
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

    public static TransactionDto? Map(Transaction? entity)
    {
        if (entity is null)
            return default;

        switch (entity.TransactionType)
        {
            case TransactionEnums.TransactionKeys.PURCHASE:
                return PopulateCommonDtoProperties<PurchaseTransactionDto>(entity);

            case TransactionEnums.TransactionKeys.DEPOSIT:
                return PopulateCommonDtoProperties<DepositTransactionDto>(entity);

            case TransactionEnums.TransactionKeys.WITHDRAWAL:
                return PopulateCommonDtoProperties<WithdrawalTransactionDto>(entity);

            default:
                throw new InvalidOperationException($"{nameof(ApplicationMapper)}: " +
                    $"'{entity.TransactionType}' is not yet support");
        }
    }

    public static TransactionItemDto? Map(TransactionItem? item)
    {
        if (item is null)
            return default;

        return new TransactionItemDto
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            SubCategoryId = item.SubCategoryId,
            Description = item.Description,
            Amount = item.Amount,
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


    private static TransactionDto PopulateCommonDtoProperties<TDto>(Transaction entity)
        where TDto : TransactionDto, new()
    {
        var transactionDto = new TDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountId = entity.AccountId,
            Description = entity.Description,
            ExtTransactionId = entity.ExtTransactionId,
            PayerPayeeId = entity.PayerPayeeId,
            RefTransactionId = entity.RefTransactionId,
            Tags = entity.Tags.ToList(),
            TransactionDate = entity.TransactionDate
        };

        if (entity is PurchaseTransaction purchaseEntity &&
            transactionDto is PurchaseTransactionDto purchaseDto)
        {
            purchaseDto.Items = Map(purchaseEntity.Items).ToList();
            purchaseDto.TotalPaid = purchaseEntity.TotalPaid;

            return purchaseDto;
        }

        transactionDto.TotalAmount = entity.TotalAmount;

        return transactionDto;
    }
}