using System.Reflection;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using Habanerio.Xpnss.Merchants.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Merchants.Application;

public static class MerchantsSetup
{
    public static IServiceCollection AddMerchantsModule(this IServiceCollection services)
    {
        // Microsoft.Extensions.Options.ConfigurationExtensions
        services.AddOptions<MongoDbSettings>()
            .BindConfiguration("XpnssMongoDBSettings");

        services.AddScoped<IMerchantsRepository, MerchantsRepository>();
        services.AddScoped<IMerchantsService, MerchantsService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<MerchantDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}