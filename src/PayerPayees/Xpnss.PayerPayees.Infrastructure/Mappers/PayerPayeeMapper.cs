using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.PayerPayees.Domain;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static PayerPayee? Map(PayerPayeeDocument? document)
    {
        if (document is null)
            return null;

        return PayerPayee.Load(
            new PayerPayeeId(document.Id.ToString()),
            new UserId(document.UserId),
            new PayerPayeeName(document.Name),
            document.Location);
    }

    public static IEnumerable<PayerPayee> Map(IEnumerable<PayerPayeeDocument> documents)
    {
        return documents.Select(Map)
            .Where(x => x is not null)
            .Cast<PayerPayee>();
    }

    public static PayerPayeeDocument? Map(PayerPayee? entity)
    {
        if (entity is null)
            return null;

        //if (string.IsNullOrWhiteSpace(entity.Id))
        //    return PayerPayeeDocument.NewId(entity.UserId, entity.Name, entity.Location);

        return new PayerPayeeDocument(entity.Id, entity.UserId, entity.Name.Value, entity.Location);
    }

    public static IEnumerable<PayerPayeeDocument> Map(IReadOnlyCollection<PayerPayee> entities)
    {
        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<PayerPayeeDocument>();
    }
}