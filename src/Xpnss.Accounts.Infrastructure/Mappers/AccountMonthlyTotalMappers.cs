using Habanerio.Xpnss.Accounts.Domain.Entities;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Mappers;

internal static partial class Mapper
{
    public static AccountMonthlyTotal? Map(AccountMonthlyTotalDocument? document)
    {
        if (document is null)
            return null;

        return AccountMonthlyTotal.Load(
            new EntityObjectId(document.Id),
            new AccountId(document.AccountId),
            new UserId(document.UserId),
            document.Year,
            document.Month,
            new Money(document.CreditTotal),
            document.CreditCount,
            new Money(document.DebitTotal),
            document.DebitCount);
    }

    public static IEnumerable<AccountMonthlyTotal> Map(IEnumerable<AccountMonthlyTotalDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<AccountMonthlyTotal>();
    }

    public static AccountMonthlyTotalDocument? Map(AccountMonthlyTotal? monthlyTotal)
    {
        if (monthlyTotal is null)
            return null;

        return new AccountMonthlyTotalDocument
        {
            Id = ObjectId.Parse(monthlyTotal.Id),
            UserId = monthlyTotal.UserId,
            AccountId = ObjectId.Parse(monthlyTotal.AccountId),
            Year = monthlyTotal.Year,
            Month = monthlyTotal.Month,
            CreditTotal = monthlyTotal.CreditTotalAmount,
            CreditCount = monthlyTotal.CreditCount,
            DebitTotal = monthlyTotal.DebitTotalAmount,
            DebitCount = monthlyTotal.DebitCount
        };
    }

    public static List<AccountMonthlyTotalDocument> Map(IEnumerable<AccountMonthlyTotal> monthlyTotals)
    {
        return monthlyTotals
            .Select(Map)
            .Where(x => x is not null)
            .Cast<AccountMonthlyTotalDocument>()
            .ToList();
    }
}