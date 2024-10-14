using Carter;
using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Accounts;
using Habanerio.Xpnss.Modules.Transactions;

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

        //builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("XpnssMongoDBSettings"));

        builder.Services.AddCarter();

        builder.Services.AddCors();

        //builder.Services.AddMongoDbContext<AccountsDbContext>();
        builder.Services.AddAccountsModule();
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