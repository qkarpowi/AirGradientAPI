var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("airgradientdb");

var apiService = builder.AddProject<Projects.AirGradientAPI>("airgradientapi")
    .WithReference(postgres);

builder.Build().Run();
