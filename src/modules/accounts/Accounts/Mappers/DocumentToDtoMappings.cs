using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;

namespace Habanerio.Xpnss.Modules.Accounts.Mappers;

public static class DocumentToDtoMappings
{
    public static IEnumerable<AccountDto> Map(IEnumerable<AccountDocument> documents)
    {
        var results = new List<AccountDto>();

        foreach (var document in documents)
        {
            var dto = Map(document);

            if (dto is not null)
                results.Add(dto);
        }

        return results;
    }

    public static AccountDto? Map(AccountDocument? document)
    {
        if (document is null)
            return default;

        switch (document.AccountType)
        {
            case AccountType.Cash:
                var cash = new CashAccountDto(
                    document.Id.ToString(),
                    document.UserId,
                    document.Name,
                    document.Description,
                    document.Balance,
                    document.DisplayColor,
                    document.DateCreated,
                    document.DateUpdated,
                    document.DateDeleted);

                MapExtendedPropertiesToDto(cash, document.ExtendedProps);

                return cash;

            case AccountType.Checking:
                var checking = new CheckingAccountDto(
                    document.Id.ToString(),
                    document.UserId,
                    document.Name,
                    document.Description,
                    document.Balance,
                    overDraftAmount: 0,
                    document.DisplayColor,
                    document.DateCreated,
                    document.DateUpdated,
                    document.DateDeleted);

                MapExtendedPropertiesToDto(checking, document.ExtendedProps);

                return checking;

            case AccountType.Savings:
                var savings = new SavingsAccountDto(
                    document.Id.ToString(),
                    document.UserId,
                    document.Name,
                    document.Description,
                    document.Balance,
                    interestRate: 0,
                    document.DisplayColor,
                    document.DateCreated,
                    document.DateUpdated,
                    document.DateDeleted);

                MapExtendedPropertiesToDto(savings, document.ExtendedProps);

                return savings;

            case AccountType.CreditCard:
                var creditCard = new CreditCardAccountDto(
                    document.Id.ToString(),
                    document.UserId,
                    document.Name,
                    document.Description,
                    document.Balance,
                    creditLimit: 0,
                    interestRate: 0,
                    document.DisplayColor,
                    dateCreated: document.DateCreated,
                    dateUpdated: document.DateUpdated,
                    dateDeleted: document.DateDeleted);

                MapExtendedPropertiesToDto(creditCard, document.ExtendedProps);

                return creditCard;

            case AccountType.LineOfCredit:
                var lineOfCredit = new LineOfCreditAccountDto(
                    document.Id.ToString(),
                    document.UserId,
                    document.Name,
                    document.Description,
                    document.Balance,
                    creditLimit: 0,
                    interestRate: 0,
                    document.DisplayColor,
                    dateCreated: document.DateCreated,
                    dateUpdated: document.DateUpdated,
                    dateDeleted: document.DateDeleted);

                MapExtendedPropertiesToDto(lineOfCredit, document.ExtendedProps);

                return lineOfCredit;

            default:
                throw new NotSupportedException("Unknown account type");
        }
    }

    private static void MapExtendedPropertiesToDto(AccountDto dto, List<KeyValuePair<string, object?>> extendedProps)
    {
        foreach (var prop in extendedProps)
        {
            var property = dto.GetType().GetProperty(prop.Key);

            if (property is not null)
                property.SetValue(dto, prop.Value);
        }
    }

    //public static AccountDocument? Map(MoneyAccount? entity)
    //{
    //    if (entity is null)
    //        return default;

    //    switch (entity)
    //    {
    //        //case CheckingAccountDto checking:
    //        //    var checkingDocument = new CheckingAccountDocument();

    //        //    MapCommonProperties(checkingDocument, checking);

    //        //    checkingDocument.OverDraftLimit = checking.OverDraftLimit;

    //        //    return checkingDocument;

    //        //case SavingsAccountDto savings:
    //        //    var savingsDocument = new SavingsAccountDocument();

    //        //    MapCommonProperties(savingsDocument, savings);

    //        //    savingsDocument.InterestRate = savings.InterestRate;

    //        //    return savingsDocument;

    //        //case CreditCardAccountDto creditCard:
    //        //    var creditCardDocument = new CreditCardAccountDocument();

    //        //    MapCommonProperties(creditCardDocument, creditCard);

    //        //    creditCardDocument.CreditLimit = creditCard.CreditLimit;
    //        //    creditCardDocument.InterestRate = creditCard.InterestRate;
    //        //    creditCardDocument.BillingCycleDays = creditCard.BillingCycleDays;

    //        //    return creditCardDocument;

    //        //case LineOfCreditAccountDto loc:
    //        //    var locDocument = new LineOfCreditAccountDocument();

    //        //    MapCommonProperties(locDocument, loc);

    //        //    locDocument.CreditLimit = loc.CreditLimit;
    //        //    locDocument.InterestRate = loc.InterestRate;

    //        //    return locDocument;

    //        default:
    //            throw new NotSupportedException("Unknown account type");
    //    }
    //}

    //private static void MapCommonProperties(MoneyAccountEntity document, MoneyAccount entity)
    //{
    //    entity.Id = document.Id.ToString();

    //    entity.UserId = document.UserId;

    //    entity.Name = document.Name;
    //    //entity.Description = document.Description;
    //    //entity.DisplayColor = document.DisplayColor;

    //    entity.Balance = document.Balance;
    //    //entity.BalanceStart = document.BalanceStart;

    //    entity.IsDefault = document.IsDefault;
    //    entity.IsDeleted = document.IsDeleted;

    //    entity.DateCreated = document.DateCreated;
    //    entity.DateUpdated = document.DateUpdated;
    //    entity.DateDeleted = document.DateDeleted;
    //}

    //private static void MapCommonProperties(MoneyAccount dto, MoneyAccountEntity document)
    //{
    //    document.Id = ObjectId.Parse(dto.Id);

    //    document.UserId = dto.UserId;

    //    document.Name = dto.Name;
    //    //document.Description = dto.Description;
    //    //document.DisplayColor = dto.DisplayColor;

    //    document.Balance = dto.Balance;
    //    //document.BalanceStart = dto.BalanceStart;

    //    document.IsDefault = dto.IsDefault;

    //    document.DateCreated = dto.DateCreated;
    //    document.DateUpdated = dto.DateUpdated;
    //    document.DateDeleted = dto.DateDeleted;
    //}
}