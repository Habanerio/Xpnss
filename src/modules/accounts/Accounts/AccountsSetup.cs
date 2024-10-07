using System.Reflection;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Modules.Accounts;

public static class AccountsSetup
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IAccountsService, AccountsService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<AccountDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            //cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<CashAccount>();
        BsonClassMap.RegisterClassMap<CheckingAccount>();
        BsonClassMap.RegisterClassMap<SavingsAccount>();
        BsonClassMap.RegisterClassMap<CreditCardAccount>();
        BsonClassMap.RegisterClassMap<LineOfCreditAccount>();

        return services;
    }
}