using System.Reflection;
using Habanerio.Xpnss.Application.Categories;
using Habanerio.Xpnss.Domain.Categories.Interfaces;
using Habanerio.Xpnss.Infrastructure.Documents;
using Habanerio.Xpnss.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Application.Setups;

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