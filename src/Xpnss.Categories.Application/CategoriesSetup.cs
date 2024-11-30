using System.Reflection;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Categories.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Categories.Application;

public static class CategoriesSetup
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services)
    {
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ICategoriesService, CategoriesService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(Infrastructure.IntegrationEvents.EventHandlers.UserProfileCreatedIntegrationEventHandler).Assembly);
        });


        BsonClassMap.RegisterClassMap<CategoryDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}