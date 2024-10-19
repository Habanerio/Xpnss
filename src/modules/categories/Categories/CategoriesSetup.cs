using System.Reflection;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Modules.Categories;

public static class CategoriesSetup
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services)
    {
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ICategoriesService, CategoriesService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<CategoryDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}