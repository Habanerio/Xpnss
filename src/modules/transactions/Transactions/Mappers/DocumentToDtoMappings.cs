using Habanerio.Xpnss.Modules.Transactions.Data;
using Habanerio.Xpnss.Modules.Transactions.DTOs;

namespace Habanerio.Xpnss.Modules.Transactions.Mappers;

public static class DocumentToDtoMappings
{
    public static IEnumerable<TransactionDto> Map(IEnumerable<TransactionDocument> documents)
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

    public static TransactionDto? Map(TransactionDocument? document)
    {
        if (document is null)
            return default;

        return new TransactionDto
        {
            Id = document.Id.ToString(),
            UserId = document.UserId,
            AccountId = document.AccountId.ToString(),
            Amount = document.TotalAmount,
            Description = document.Description,
            Items = Map(document.Items.AsEnumerable()).ToList().AsReadOnly(),
            Merchant = Map(document.Merchant),
            Paid = document.TotalPaid,
            TransactionDate = document.TransactionDate,
            TransactionType = document.TransactionTypes.ToString(),
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
            Id = item.Id.ToString(),
            //TransactionId = item.TransactionId.ToString(),
            //ItemId = item.Id.ToString(),
            CategoryId = item.CategoryId?.ToString() ?? string.Empty,
            Description = item.Description,
            Amount = item.Amount,
        };
    }

    public static MerchantDto Map(TransactionMerchant? merchant)
    {
        if (merchant is null)
            return new MerchantDto();

        return new MerchantDto
        {
            Id = merchant.Id.ToString(),
            Name = merchant.Name,
            Location = merchant.Location,
        };
    }
}