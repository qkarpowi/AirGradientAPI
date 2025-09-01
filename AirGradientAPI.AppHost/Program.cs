var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AirGradientAPI>("airgradientapi");

builder.Build().Run();
