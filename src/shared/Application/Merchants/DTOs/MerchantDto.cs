namespace Habanerio.Xpnss.Application.Merchants.DTOs;

public record MerchantDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    //public required List<MonthlyMerchantTotalsDto> MonthlyTotal { get; set; }

    //[JsonConstructor]
    public MerchantDto(string id, string userId, string name, string location)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Location = location;
    }
}