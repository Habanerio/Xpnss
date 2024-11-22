using System.Reflection;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Transactions.Application;

public static class TransactionsSetup
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
        // Microsoft.Extensions.Options.ConfigurationExtensions
        services.AddOptions<MongoDbSettings>()
            .BindConfiguration("XpnssMongoDBSettings");

        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
        services.AddScoped<ITransactionsService, TransactionsService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        //services.AddScoped<IRequestHandler<TransactionCreatedDomainEvent>, TransactionCreatedDomainEventHandler>();

        BsonClassMap.RegisterClassMap<TransactionDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}