using Carter;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Apis.App.AppApis.Managers;
using Habanerio.Xpnss.Modules.Accounts;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Categories;
using Habanerio.Xpnss.Modules.Transactions;
using Microsoft.AspNetCore.Http.Json;

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

        builder.Services.AddOptions<MongoDbSettings>()
        .BindConfiguration("XpnssMongoDBSettings");

        builder.Services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.Converters.Add(new AccountDtoConverter());
        });

        builder.Services.AddCarter();

        builder.Services.AddCors();

        builder.Services.AddScoped<IAccountTransactionManager, AccountTransactionManager>();

        builder.Services.AddAccountsModule();
        builder.Services.AddCategoriesModule();
        builder.Services.AddTransactionsModule();

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