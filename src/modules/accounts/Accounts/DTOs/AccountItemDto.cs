using Habanerio.Xpnss.Modules.Accounts.Common;

namespace Habanerio.Xpnss.Modules.Accounts.DTOs;

public record AccountItemDto
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public AccountTypes AccountTypes { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public string DisplayColor { get; set; }

    public bool IsCredit { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    protected AccountItemDto(AccountTypes accountTypes, bool isCredit)
    {
        AccountTypes = accountTypes;
        IsCredit = isCredit;
    }
}