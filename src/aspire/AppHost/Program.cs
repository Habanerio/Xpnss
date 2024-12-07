var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AppApis>("appapis");

builder.AddProject<Projects.XpnssWeb>("xpnssweb");

await builder.Build().RunAsync();
