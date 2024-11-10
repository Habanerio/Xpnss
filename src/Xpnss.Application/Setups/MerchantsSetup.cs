using System.Reflection;
using Habanerio.Xpnss.Application.Merchants;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Application.Setups;

public static class MerchantsSetup
{
    public static IServiceCollection AddMerchantsModule(this IServiceCollection services)
    {
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