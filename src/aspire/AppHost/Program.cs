using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// https://learn.microsoft.com/en-us/dotnet/aspire/caching/stackexchange-redis-integration?tabs=dotnet-cli&pivots=redis
var redis_web = builder
    .AddRedis("xpnss-web-redis");
// .WithDataVolume()


var redis_api = builder
    .AddRedis("xpnss-apis-redis")
    .WithRedisCommander();

var api = builder
    .AddProject<AppApis>("xpnss-apis")
    .WithReference(redis_api);

builder.AddProject<XpnssWeb>("xpnssweb")
    .WithReference(redis_web)
    .WithReference(api);

await builder.Build().RunAsync();
