namespace AirGradientAPI.Models;

using Microsoft.EntityFrameworkCore;
using AirGradientAPI.Entities;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<SensorDatum> SensorData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorDatum>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // ChipId configuration
            entity.Property(e => e.ChipId)
                .IsRequired()
                .HasMaxLength(50);

            // Timestamp configuration
            entity.Property(e => e.Timestamp)
                .HasColumnName("Timestamp");

            // Performance indexes
            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_SensorData_Timestamp");

            // Additional indexes for sensor readings if needed for analytics
            entity.HasIndex(e => e.Rco2)
                .HasDatabaseName("IX_SensorData_CO2");
        });
    }
}