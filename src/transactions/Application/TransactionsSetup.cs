using System.Reflection;
using System.Text.Json;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Transactions.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Transactions.Application;

public static class TransactionsSetup
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
        //CreateTransactionRequestsJsonConverter
        services.Configure<JsonOptions>(opt =>
        {
            //opt.SerializerOptions.PropertyNameCaseInsensitive = true;
            opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opt.SerializerOptions.Converters.Add(new CreateTransactionRequestsJsonConverter());
            opt.SerializerOptions.Converters.Add(new TransactionDtoJsonConverter());
        });

        // Microsoft.Extensions.Options.ConfigurationExtensions
        services.AddOptions<MongoDbSettings>()
            .BindConfiguration("XpnssMongoDBSettings");

        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
        services.AddScoped<ITransactionsService, TransactionsService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<TransactionDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<CreditTransactionDocument>();
        BsonClassMap.RegisterClassMap<DebitTransactionDocument>();
        //BsonClassMap.RegisterClassMap<DepositTransactionDocument>();
        //BsonClassMap.RegisterClassMap<PaymentTransactionDocument>();
        BsonClassMap.RegisterClassMap<PurchaseTransactionDocument>();
        //BsonClassMap.RegisterClassMap<WithdrawalTransactionDocument>();

        return services;
    }
}