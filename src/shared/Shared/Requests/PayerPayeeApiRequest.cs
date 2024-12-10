namespace Habanerio.Xpnss.Shared.Requests;

public sealed record PayerPayeeApiRequest
{
    /// <summary>
    /// The id of an existing PayerPayee.
    /// If the PayerPayee doesn't exist, then leave empty and fill out name
    /// </summary>
    public string Id { get; init; } = string.Empty;


    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;
}