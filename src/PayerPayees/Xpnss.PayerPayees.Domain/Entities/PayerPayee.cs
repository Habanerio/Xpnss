using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.PayerPayees.Domain.Entities;

public sealed class PayerPayee : AggregateRoot<PayerPayeeId>
{
    public UserId UserId { get; set; }

    public PayerPayeeName Name { get; set; }

    public string Description { get; set; }

    public string Location { get; set; }

    private PayerPayee(
        UserId userId,
        PayerPayeeName name,
        string description,
        string location) :
        this(
            PayerPayeeId.Empty,
            userId,
            name,
            description,
            location,
            DateTime.UtcNow)
    {
        IsTransient = true;

        // Add `MonthlyTotalsCreated` Domain Event
    }

    private PayerPayee(
        PayerPayeeId id,
        UserId userId,
        PayerPayeeName name,
        string description,
        string location,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Location = location;

        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }

    public static PayerPayee Load(
        PayerPayeeId id,
        UserId userId,
        PayerPayeeName name,
        string description,
        string location,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new PayerPayee(id, userId, name, description, location, dateCreated, dateUpdated, dateDeleted);
    }

    public static PayerPayee New(
        UserId userId,
        PayerPayeeName name,
        string description,
        string location)
    {
        return new PayerPayee(userId, name, description, location);
    }
}