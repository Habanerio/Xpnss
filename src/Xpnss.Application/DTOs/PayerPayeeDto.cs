namespace Habanerio.Xpnss.Application.DTOs;

public sealed record PayerPayeeDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Location { get; set; }

    //public required List<MonthlyMerchantTotalsDto> BaseMonthlyTotalDocument { get; set; }

    //[JsonConstructor]
    public PayerPayeeDto(string id, string userId, string name, string description, string location)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        Location = location;
    }
}