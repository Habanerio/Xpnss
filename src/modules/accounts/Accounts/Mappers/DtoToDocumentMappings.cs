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
            AccountType = accountDto.AccountType,
            Name = accountDto.Name,
            Description = accountDto.Description,
            Balance = accountDto.Balance,
            DisplayColor = accountDto.DisplayColor,
            DateCreated = accountDto.DateCreated,
            DateUpdated = accountDto.DateUpdated,
            DateDeleted = accountDto.DateDeleted
        };

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