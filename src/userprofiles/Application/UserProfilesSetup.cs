using System.Reflection;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Documents;
using Habanerio.Xpnss.UserProfiles.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.UserProfiles.Application;

public static class UserProfilesSetup
{
    public static IServiceCollection AddUserProfilesModule(this IServiceCollection services)
    {
        services.AddScoped<IUserProfilesRepository, UserProfilesRepository>();
        services.AddScoped<IUserProfilesService, UserProfilesService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<UserProfileDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<UserSettingsDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}