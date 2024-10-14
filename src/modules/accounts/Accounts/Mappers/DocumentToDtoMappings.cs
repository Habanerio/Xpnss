using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;

namespace Habanerio.Xpnss.Modules.Accounts.Mappers;

public static class DocumentToDtoMappings
{
    /// <summary>
    /// This will map a list of documents to a list of dtos.
    /// If any of the mapped dtos are null, they will be skipped.
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
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

    /// <summary>
    /// This is for when you have a single document to map.
    /// Do NOT iterate over this. Use <see cref="Map(IEnumerable{AccountDocument})"/> instead.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static AccountDto? Map(AccountDocument? document)
    {
        if (document is null)
            return default;

        switch (document.AccountTypes)
        {
            case AccountTypes.Cash:
                var cashDoc = document as CashAccount;
                var cash = new CashAccountDto(
                    cashDoc.Id.ToString(),
                    cashDoc.UserId,
                    cashDoc.Name,
                    cashDoc.Description,
                    cashDoc.Balance,
                    cashDoc.DisplayColor,
                    cashDoc.DateCreated,
                    cashDoc.DateUpdated,
                    cashDoc.DateDeleted);

                return cash;

            case AccountTypes.Checking:
                var checkingDoc = document as CheckingAccount;
                var checking = new CheckingAccountDto(
                    checkingDoc.Id.ToString(),
                    checkingDoc.UserId,
                    checkingDoc.Name,
                    checkingDoc.Description,
                    checkingDoc.Balance,
                    overDraftAmount: checkingDoc.OverDraftAmount,
                    checkingDoc.DisplayColor,
                    checkingDoc.DateCreated,
                    checkingDoc.DateUpdated,
                    checkingDoc.DateDeleted);

                return checking;

            case AccountTypes.Savings:
                var savingsDoc = document as SavingsAccount;
                var savings = new SavingsAccountDto(
                    savingsDoc.Id.ToString(),
                    savingsDoc.UserId,
                    savingsDoc.Name,
                    savingsDoc.Description,
                    savingsDoc.Balance,
                    interestRate: savingsDoc.InterestRate,
                    savingsDoc.DisplayColor,
                    savingsDoc.DateCreated,
                    savingsDoc.DateUpdated,
                    savingsDoc.DateDeleted);

                return savings;

            case AccountTypes.CreditCard:
                var creditCardDoc = document as CreditCardAccount;
                var creditCard = new CreditCardAccountDto(
                    creditCardDoc.Id.ToString(),
                    creditCardDoc.UserId,
                    creditCardDoc.Name,
                    creditCardDoc.Description,
                    creditCardDoc.Balance,
                    creditLimit: creditCardDoc.CreditLimit,
                    interestRate: creditCardDoc.InterestRate,
                    creditCardDoc.DisplayColor,
                    dateCreated: creditCardDoc.DateCreated,
                    dateUpdated: creditCardDoc.DateUpdated,
                    dateDeleted: creditCardDoc.DateDeleted);

                return creditCard;

            case AccountTypes.LineOfCredit:
                var locDoc = document as LineOfCreditAccount;
                var lineOfCredit = new LineOfCreditAccountDto(
                    locDoc.Id.ToString(),
                    locDoc.UserId,
                    locDoc.Name,
                    locDoc.Description,
                    locDoc.Balance,
                    creditLimit: locDoc.CreditLimit,
                    interestRate: locDoc.InterestRate,
                    locDoc.DisplayColor,
                    dateCreated: locDoc.DateCreated,
                    dateUpdated: locDoc.DateUpdated,
                    dateDeleted: locDoc.DateDeleted);

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

    //        //    creditCardDocument.InterestRate = creditCard.InterestRate;
    //        //    creditCardDocument.InterestRate = creditCard.InterestRate;
    //        //    creditCardDocument.BillingCycleDays = creditCard.BillingCycleDays;

    //        //    return creditCardDocument;

    //        //case LineOfCreditAccountDto loc:
    //        //    var locDocument = new LineOfCreditAccountDocument();

    //        //    MapCommonProperties(locDocument, loc);

    //        //    locDocument.InterestRate = loc.InterestRate;
    //        //    locDocument.InterestRate = loc.InterestRate;

    //        //    return locDocument;

    //        default:
    //            throw new NotSupportedException("Unknown account type");
    //    }
    //}

    //private static void MapCommonProperties(MoneyAccountEntity document, MoneyAccount entity)
    //{
    //    entity.AccountId = document.AccountId.ToString();

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
    //    document.AccountId = ObjectId.Parse(dto.AccountId);

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