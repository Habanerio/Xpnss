var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AppApis>("appapis");

builder.AddProject<Projects.AdminApis>("adminapis");

builder.Build().Run();
