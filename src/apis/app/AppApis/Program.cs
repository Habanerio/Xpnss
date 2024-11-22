using Carter;
using Habanerio.Xpnss.Accounts.Application;
using Habanerio.Xpnss.Categories.Application;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
using Habanerio.Xpnss.Infrastructure.Interfaces;
using Habanerio.Xpnss.Merchants.Application;
using Habanerio.Xpnss.Transactions.Application;

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

        //builder.Services.AddOptions<MongoDbSettings>()
        //.BindConfiguration("XpnssMongoDBSettings");

        builder.Services.AddCarter();

        builder.Services.AddCors();


        builder.Services.AddAccountsModule();
        builder.Services.AddCategoriesModule();
        builder.Services.AddMerchantsModule();
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