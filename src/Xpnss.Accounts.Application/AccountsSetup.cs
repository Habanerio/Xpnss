using System.Reflection;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Accounts.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.Accounts.Application;

public static class AccountsSetup
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.PropertyNameCaseInsensitive = true;
            opt.SerializerOptions.Converters.Add(new AccountRequestJsonConverter());
        });

        services.AddScoped<IAccountsRepository, AccountsRepository>();

        services.AddScoped<IAccountsService, AccountsService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(
                typeof(Infrastructure.IntegrationEvents.EventHandlers.TransactionCreatedIntegrationEventHandler).Assembly);
        });

        // Setup Accounts documents
        BsonClassMap.RegisterClassMap<AccountDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetDiscriminator("_t");
            cm.SetDiscriminatorIsRequired(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}