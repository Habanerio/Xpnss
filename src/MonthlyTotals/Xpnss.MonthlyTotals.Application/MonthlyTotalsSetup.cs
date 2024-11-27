using System.Reflection;
using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Documents;
using Habanerio.Xpnss.MonthlyTotals.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.MonthlyTotals.Application;

public static class MonthlyTotalsSetup
{
    public static IServiceCollection AddMonthlyTotalsModule(this IServiceCollection services)
    {
        services.AddScoped<IMonthlyTotalsRepository, MonthlyTotalsRepository>();
        services.AddScoped<IMonthlyTotalsService, MonthlyTotalsService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(Infrastructure.IntegrationEvents.EventHandlers.TransactionCreatedIntegrationEventHandler).Assembly);
        });

        BsonClassMap.RegisterClassMap<MonthlyTotalDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}