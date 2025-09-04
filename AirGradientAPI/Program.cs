using AirGradientAPI;
using AirGradientAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
if (builder.Environment.IsDevelopment())
{
    // Use Aspire PostgreSQL in development
    builder.AddNpgsqlDbContext<DataContext>("airgradientdb");
}
else
{
    // Use connection string from environment variables in production
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? throw new InvalidOperationException("Connection string not found. Set CONNECTION_STRING environment variable or DefaultConnection in appsettings.");
    
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AirGradient API",
        Version = "v1",
        Description = "API for receiving and storing AirGradient sensor data including WiFi signal strength, CO2 levels, PM2.5 particles, temperature, and humidity measurements.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AirGradient API Support"
        }
    });
    
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();

public partial class Program { }