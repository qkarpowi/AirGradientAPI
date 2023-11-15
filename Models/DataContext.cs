namespace AirGradientAPI.Models;

using Microsoft.EntityFrameworkCore;
using AirGradientAPI.Entities;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<SensorDatum> SensorData { get; set; }
}