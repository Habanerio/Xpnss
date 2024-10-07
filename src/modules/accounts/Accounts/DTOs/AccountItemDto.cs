using Habanerio.Xpnss.Modules.Accounts.Common;

namespace Habanerio.Xpnss.Modules.Accounts.DTOs;

public record AccountItemDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public AccountType AccountType { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public string DisplayColor { get; set; }

    public bool IsCredit { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    protected AccountItemDto(AccountType accountType, bool isCredit)
    {
        AccountType = accountType;
        IsCredit = isCredit;
    }
}