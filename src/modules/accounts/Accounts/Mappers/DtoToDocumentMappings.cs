using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Modules.Accounts.Mappers;

public static class DtoToDocumentMappings
{
    public static AccountDocument? Map(AccountDto? accountDto)
    {
        if (accountDto is null)
            return default;

        var doc = new AccountDocument()
        {
            Id = ObjectId.Parse(accountDto.Id),
            UserId = accountDto.UserId,
            Name = accountDto.Name,
            Description = accountDto.Description,
            Balance = accountDto.Balance,
            DisplayColor = accountDto.DisplayColor,
            DateCreated = accountDto.DateCreated,
            DateUpdated = accountDto.DateUpdated,
            DateDeleted = accountDto.DateDeleted
        };

        var extendedProps = new List<KeyValuePair<string, object?>>();

        foreach (var prop in accountDto.GetType().GetProperties())
        {
            if (string.IsNullOrWhiteSpace(prop.Name) ||
                prop.Name == nameof(AccountDto.Id) ||
                prop.Name == nameof(AccountDto.UserId) ||
                prop.Name == nameof(AccountDto.Name) ||
                prop.Name == nameof(AccountDto.AccountType) ||
                prop.Name == nameof(AccountDto.Balance) ||
                prop.Name == nameof(AccountDto.Description) ||
                prop.Name == nameof(AccountDto.DisplayColor) ||
                prop.Name == nameof(AccountDto.IsCredit) ||
                prop.Name == nameof(AccountDto.IsDeleted) ||
                //prop.Name == nameof(AccountDto.ChangeHistory) ||
                prop.Name == nameof(AccountDto.DateCreated) ||
                prop.Name == nameof(AccountDto.DateUpdated) ||
                prop.Name == nameof(AccountDto.DateDeleted)
                // || prop.Name == nameof(AccountDto.ChangeHistoryItems)
                )
                continue;

            var value = prop.GetValue(accountDto) ?? default;

            extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, value));
        }

        doc.ExtendedProps = extendedProps;

        return doc;
    }

    public static IEnumerable<AccountDocument> Map(IEnumerable<AccountDto> accountDtos)
    {
        var results = new List<AccountDocument>();

        foreach (var dto in accountDtos)
        {
            var doc = Map(dto);

            if (doc is not null)
                results.Add(doc);
        }

        return results;
    }
}