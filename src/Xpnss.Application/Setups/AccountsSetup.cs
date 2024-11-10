using System.Reflection;
using Habanerio.Xpnss.Application.Accounts;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Application.Setups;

public static class AccountsSetup
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IAccountsService, AccountsService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        //services.Configure<JsonOptions>(o =>
        //{
        //    o.SerializerOptions.Converters.Add(new AccountDtoConverter());
        //});

        BsonClassMap.RegisterClassMap<AccountDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<CashAccountDocument>();
        BsonClassMap.RegisterClassMap<CheckingAccountDocument>();
        BsonClassMap.RegisterClassMap<SavingsAccountDocument>();
        BsonClassMap.RegisterClassMap<CreditCardAccountDocument>();
        BsonClassMap.RegisterClassMap<LineOfCreditAccountDocument>();

        return services;
    }
}