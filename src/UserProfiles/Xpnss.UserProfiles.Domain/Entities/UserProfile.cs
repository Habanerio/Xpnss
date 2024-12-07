using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.UserProfiles.Domain.Entities;

public class UserProfile : AggregateRoot<UserId>
{
    /// <summary>
    /// User's Id from an external authentication provider
    /// </summary>
    public string ExtUserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public CurrencyEnums.CurrencyKeys DefaultCurrency { get; }

    public DateTime DateLastSeen { get; set; }

    // New User Profiles
    private UserProfile(
        string extUserId,
        string firstName,
        string lastName,
        string email,
        CurrencyEnums.CurrencyKeys defaultCurrency = CurrencyEnums.CurrencyKeys.CAD) :
        this(
            UserId.New,
            extUserId,
            firstName,
            lastName,
            email,
            defaultCurrency,
            DateTime.UtcNow,
            DateTime.UtcNow)
    {
        IsTransient = true;

        // AddDomainEvent();
    }

    // Existing User Profiles
    private UserProfile(
        UserId id,
        string extUserId,
        string firstName,
        string lastName,
        string email,
        CurrencyEnums.CurrencyKeys defaultCurrency,
        DateTime dateLastSeen,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        ExtUserId = extUserId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        DefaultCurrency = defaultCurrency;
        DateLastSeen = dateLastSeen;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }

    public static UserProfile Load(
        UserId id,
        string extUserId,
        string firstName,
        string lastName,
        string email,
        CurrencyEnums.CurrencyKeys defaultCurrency,
        DateTime dateLastSeen,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new UserProfile(
            id,
            extUserId,
            firstName,
            lastName,
            email,
            defaultCurrency,
            dateLastSeen,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static UserProfile New(
        string extUserId,
        string firstName,
        string lastName,
        string email,
        CurrencyEnums.CurrencyKeys defaultCurrency)
    {
        return new UserProfile(extUserId, firstName, lastName, email, defaultCurrency);
    }
}