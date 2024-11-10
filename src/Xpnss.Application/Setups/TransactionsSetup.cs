using System.Reflection;
using Habanerio.Xpnss.Application.Transactions;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Application.Setups;

public static class TransactionsSetup
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
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