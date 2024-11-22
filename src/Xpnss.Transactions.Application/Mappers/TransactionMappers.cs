using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Transactions.Domain;

namespace Habanerio.Xpnss.Transactions.Application.Mappers;

internal static partial class Mapper
{
    public static IEnumerable<TransactionDto> Map(IEnumerable<Transaction> documents)
    {
        var results = new List<TransactionDto>();

        foreach (var document in documents)
        {
            var dto = Map(document);

            if (dto is not null)
                results.Add(dto);
        }

        return results;
    }

    public static TransactionDto? Map(Transaction? document)
    {
        if (document is null)
            return default;

        return new TransactionDto
        {
            Id = document.Id,
            UserId = document.UserId,
            AccountId = document.AccountId,
            TotalAmount = document.TotalAmount,
            Description = document.Description,
            IsCredit = TransactionTypes.IsCreditTransaction(document.TransactionType),
            Items = Map(document.Items.AsEnumerable()).ToList().AsReadOnly(),
            MerchantId = document.MerchantId?.Value ?? null,
            Paid = document.TotalPaid,
            TransactionDate = document.TransactionDate,
            TransactionType = document.TransactionType.ToString(),
            DatePaid = document.DatePaid,
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