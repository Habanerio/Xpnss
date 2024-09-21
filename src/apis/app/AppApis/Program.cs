using Carter;

using Habanerio.Core.DBs.MongoDB.EFCore;

var userId = "0dab2540287b4467e54ddb3e";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<MongoDbSettings>()
    .BindConfiguration("XpnssMongoDBSettings");

builder.Services.AddCarter();

builder.Services.AddCors();


builder.Services.AddServiceDiscovery();

// Add services to the container.
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

await app.RunAsync();