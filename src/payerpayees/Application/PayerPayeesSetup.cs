using System.Reflection;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Documents;
using Habanerio.Xpnss.PayerPayees.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Habanerio.Xpnss.PayerPayees.Application;

public static class PayerPayeesSetup
{
    public static IServiceCollection AddPayerPayeesModule(this IServiceCollection services)
    {
        services.AddScoped<IPayerPayeesRepository, PayerPayeesRepository>();
        services.AddScoped<IPayerPayeesService, PayerPayeesService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        BsonClassMap.RegisterClassMap<PayerPayeeDocument>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.SetIgnoreExtraElements(true);
        });

        return services;
    }
}