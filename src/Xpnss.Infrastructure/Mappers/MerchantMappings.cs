using Habanerio.Xpnss.Domain.Merchants;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.Documents;

namespace Habanerio.Xpnss.Infrastructure.Mappers;

internal static partial class Mapper
{
    public static Merchant? Map(MerchantDocument? document)
    {
        if (document is null)
            return null;

        return Merchant.Load(
            new MerchantId(document.Id.ToString()),
            new UserId(document.UserId),
            new MerchantName(document.Name),
            document.Location);
    }

    public static IEnumerable<Merchant> Map(IEnumerable<MerchantDocument> documents)
    {
        return documents.Select(Map).Where(x => x is not null).Cast<Merchant>();
    }

    public static MerchantDocument? Map(Merchant? merchant)
    {
        if (merchant is null)
            return null;

        //if (string.IsNullOrWhiteSpace(merchant.Id))
        //    return MerchantDocument.New(merchant.UserId, merchant.Name, merchant.Location);

        return new MerchantDocument(merchant.Id, merchant.UserId, merchant.Name.Value, merchant.Location);
    }

    public static List<MerchantDocument> Map(IReadOnlyCollection<Merchant> merchants)
    {
        return merchants.Select(Map).Where(x => x is not null).Cast<MerchantDocument>().ToList();
    }
}