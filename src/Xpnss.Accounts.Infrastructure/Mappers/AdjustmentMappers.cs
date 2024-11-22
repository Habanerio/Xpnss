using Habanerio.Xpnss.Accounts.Domain.Entities;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Mappers;

internal static partial class Mapper
{
    public static Adjustment? Map(AdjustmentDocument? doc)
    {
        if (doc is null)
            return null;

        return Adjustment.Load(
            new EntityObjectId(doc.Id),
            new AccountId(doc.AccountId),
            new UserId(doc.UserId),
            doc.Property,
            doc.Value,
            doc.DateChanged,
            doc.Reason);
    }

    public static IEnumerable<Adjustment> Map(IEnumerable<AdjustmentDocument> docs)
    {
        return docs.Select(Map).Where(x => x is not null).Select(x => x!);
    }

    public static AdjustmentDocument? Map(Adjustment? entity)
    {
        if (entity is null)
            return null;

        return new AdjustmentDocument
        {
            AccountId = entity.AccountId,
            UserId = entity.UserId,
            Value = entity.Value,
            Property = entity.Property,
            Reason = entity.Reason,
            DateChanged = entity.DateChanged
        };
    }

    public static IEnumerable<AdjustmentDocument> Map(IEnumerable<Adjustment> entities)
    {
        return entities.Select(Map).Where(x => x is not null).Select(x => x!);
    }
}