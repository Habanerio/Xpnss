using System.Reflection;

using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Application;

public static class AccountsSetup
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        // Microsoft.Extensions.Options.ConfigurationExtensions
        services.AddOptions<MongoDbSettings>()
            .BindConfiguration("XpnssMongoDBSettings");

        // Set up Mongo, so that we can wrap MongoDb transactions with the `IClientSessionHandle`
        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.DatabaseName);
        });

        services.AddScoped<IClientSessionHandle>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.StartSession();
        });
        // End of Mongo setup

        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IAccountMonthlyTotalsRepository, AccountMonthlyTotalsRepository>();
        services.AddScoped<IAdjustmentsRepository, AdjustmentsRepository>();

        services.AddScoped<IAccountsService, AccountsService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(Infrastructure.IntegrationEvents.EventHandlers.TransactionCreatedIntegrationEventHandler).Assembly);
        });

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

        BsonClassMap.RegisterClassMap<AccountMonthlyTotalDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<AdjustmentDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}