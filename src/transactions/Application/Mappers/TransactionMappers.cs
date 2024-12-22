using Habanerio.Xpnss.Shared.DTOs;
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
            { TransactionEnums.TransactionKeys.PURCHASE, typeof(PurchasesTransactionDto) },
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

        if (entity is CreditTransaction creditEntity)
        {
            // Payments received from some other account
            if (creditEntity is PaymentInTransaction paymentInEntity)
            {
                return PopulateCommonProperties<PaymentInTransactionDto>(paymentInEntity);
            }

            return PopulateCommonProperties<CreditTransactionDto>(creditEntity);
        }

        if (entity is DebitTransaction debitEntity)
        {
            // Payments sent out to some other account
            if (debitEntity is PaymentOutTransaction paymentOutEntity)
            {
                return PopulateCommonProperties<PaymentOutTransactionDto>(paymentOutEntity);
            }


            if (debitEntity is PurchasesTransaction purchasesEntity)
            {
                return PopulateCommonProperties<PurchasesTransactionDto>(purchasesEntity);
            }

            return PopulateCommonProperties<DebitTransactionDto>(debitEntity);
        }

        throw new InvalidOperationException($"{nameof(ApplicationMapper)}: " +
            $"'{entity.TransactionType}' is not yet support");
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


    private static TransactionDto PopulateCommonProperties<TDto>(Transaction entity)
        where TDto : TransactionDto, new()
    {
        var transactionDto = new TDto
        {
            Id = entity.Id.Value,
            UserId = entity.UserId.Value,
            AccountId = entity.AccountId.Value,
            CategoryId = entity.CategoryId.Value,
            Description = entity.Description,
            ExtTransactionNo = entity.ExtTransactionNo,
            IsCredit = entity.IsCredit,
            PayerPayeeId = entity.PayerPayeeId.Value,
            SubCategoryId = entity.SubCategoryId.Value,
            //RefTransactionId = entity.RefTransactionId,
            Tags = entity.Tags.ToList(),
            TotalAmount = entity.TotalAmount,
            TransactionDate = entity.TransactionDate,
            TransactionType = entity.TransactionType
        };

        if (entity is PaymentInTransaction paymentInTransaction &&
            transactionDto is PaymentInTransactionDto paymentInDto)
        {
            paymentInDto.IsOwnAccount = paymentInTransaction.IsOwnAccount;
        }

        if (entity is PaymentOutTransaction paymentOutTransaction &&
            transactionDto is PaymentOutTransactionDto paymentOutDto)
        {
            paymentOutDto.IsOwnAccount = paymentOutTransaction.IsPaidToOwnAccount;
        }

        if (entity is PurchasesTransaction purchaseEntity &&
            transactionDto is PurchasesTransactionDto purchaseDto)
        {
            purchaseDto.Items = Map(purchaseEntity.Items).ToList();
            purchaseDto.TotalPaid = purchaseEntity.TotalPaid;

            return purchaseDto;
        }

        if (entity is CreditTransaction creditEntity &&
            transactionDto is CreditTransactionDto creditDto)
        {
            creditDto.TransactionType = entity.TransactionType;
            return creditDto;
        }

        if (entity is DebitTransaction debitEntity &&
                 transactionDto is DebitTransactionDto debitDto)
        {
            return debitDto;
        }

        transactionDto.TotalAmount = entity.TotalAmount;

        return transactionDto;
    }
}