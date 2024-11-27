using Carter;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Accounts.Application;
using Habanerio.Xpnss.Categories.Application;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
using Habanerio.Xpnss.Infrastructure.Interfaces;
using Habanerio.Xpnss.MonthlyTotals.Application;
using Habanerio.Xpnss.PayerPayees.Application;
using Habanerio.Xpnss.Transactions.Application;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Apis.App.AppApis;

// Need to wrap in 'Program' to access it from tests (?)
public class Program
{
    public static void Main(string[] args)
    {
        var userId = "0dab2540287b4467e54ddb3e";

        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddServiceDiscovery();

        builder.Services.AddCarter();

        builder.Services.AddCors();

        // Microsoft.Extensions.Options.ConfigurationExtensions
        builder.Services.AddOptions<MongoDbSettings>()
            .BindConfiguration("XpnssMongoDBSettings");

        // Set up Mongo, so that we can wrap MongoDb transactions with the `IClientSessionHandle`
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.ConnectionString);
        });

        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.DatabaseName);
        });

        builder.Services.AddScoped<IClientSessionHandle>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.StartSession();
        });
        // End of Mongo setup



        builder.Services.AddMonthlyTotalsModule();

        builder.Services.AddAccountsModule();
        builder.Services.AddCategoriesModule();
        builder.Services.AddPayerPayeesModule();
        builder.Services.AddTransactionsModule();

        builder.Services.AddScoped<IEventDispatcher, IntegrationEventDispatcher>();

        // AddDocument services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddProblemDetails();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapCarter();

        app.UseHttpsRedirection();

        app.Run();

        // If this wasn't wrapped in 'Program', we could await it here
        // await app.RunAsync();
    }
}