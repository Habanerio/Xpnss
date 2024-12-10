using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;

namespace Habanerio.Xpnss.PayerPayees.Infrastructure.Mappers;

internal static partial class InfrastructureMapper
{
    public static PayerPayee? Map(PayerPayeeDocument? document)
    {
        if (document is null)
            return default;

        return PayerPayee.Load(
            new PayerPayeeId(document.Id.ToString()),
            new UserId(document.UserId),
            new PayerPayeeName(document.Name),
            document.Description,
            document.Location,
            document.DateCreated,
            document.DateUpdated,
            document.DateDeleted);
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
            return default;

        return new PayerPayeeDocument
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            Location = entity.Location,
            DateCreated = entity.DateCreated,
            DateUpdated = entity.DateUpdated,
            DateDeleted = entity.DateDeleted
        };
    }

    public static IEnumerable<PayerPayeeDocument> Map(IReadOnlyCollection<PayerPayee> entities)
    {
        return entities.Select(Map)
            .Where(x => x is not null)
            .Cast<PayerPayeeDocument>();
    }
}