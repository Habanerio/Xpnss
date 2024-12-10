using AutoFixture;
using AutoFixture.AutoMoq;
using Habanerio.Xpnss.Shared.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Unit.Domain;

public class TestsBase
{
    protected IFixture AutoFixture;

    public TestsBase()
    {
        AutoFixture = new Fixture().Customize(new AutoMoqCustomization());
        AutoFixture.Register(NewAccountId);
        AutoFixture.Register(NewCategoryId);
        AutoFixture.Register(NewUserId);
        AutoFixture.Register(ObjectId.GenerateNewId);
    }


    public static AccountId NewAccountId()
    {
        return AccountId.New;
    }

    public static AccountName NewAccountName()
    {
        return new AccountName(Guid.NewGuid().ToString());
    }


    public static CategoryId NewCategoryId()
    {
        return CategoryId.New;
    }

    public static CategoryName NewCategoryName()
    {
        return new CategoryName(Guid.NewGuid().ToString());
    }

    public static Money NewMoney(decimal value)
    {
        return new Money(value);
    }

    public static PercentageRate NewPercentageRate(decimal value)
    {
        return new PercentageRate(value);
    }

    public static UserId NewUserId()
    {
        return UserId.New;
    }
}