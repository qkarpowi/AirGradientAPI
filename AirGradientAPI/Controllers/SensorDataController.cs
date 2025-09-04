using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Diagnostics;
using AirGradientAPI.Models;
using AirGradientAPI.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace AirGradientAPI.Controllers;

[ApiController]
[Route("api/v1/sensors")]
[Produces("application/json")]
[SwaggerTag("Sensor data collection endpoints for AirGradient devices")]
public class SensorController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<SensorController> _logger;
    private static readonly ActivitySource ActivitySource = new("AirGradientAPI.SensorController");
    
    public SensorController(DataContext context, ILogger<SensorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("airgradient:{chipId}/measures")]
    [SwaggerOperation(
        Summary = "Receive sensor data from AirGradient device",
        Description = "Accepts sensor measurements from an AirGradient device including WiFi signal strength, CO2 levels, PM2.5 particles, temperature, and humidity. The data is validated and stored in the database with a timestamp.",
        OperationId = nameof(ReceiveSensorData),
        Tags = ["Sensor Data"])]
    [SwaggerResponse(200, "Sensor data received and stored successfully", typeof(object))]
    [SwaggerResponse(400, "Invalid input data or chipId validation failed", typeof(object))]
    [SwaggerResponse(500, "Internal server error occurred while processing the data", typeof(object))]
    public async Task<IActionResult> ReceiveSensorData(
        [FromRoute, SwaggerParameter("Unique identifier for the AirGradient device (max 50 characters)", Required = true)] string chipId, 
        [FromBody, SwaggerParameter("Sensor measurement data from the AirGradient device", Required = true)] SensorDataModel sensorData)
    {
        using var activity = ActivitySource.StartActivity("SensorController.ReceiveSensorData");
        activity?.SetTag("sensor.chipId", chipId);
        activity?.SetTag("sensor.hasData", sensorData != null);

        // Validate chipId
        if (string.IsNullOrWhiteSpace(chipId))
        {
            activity?.SetTag("validation.chipId", "empty");
            activity?.SetStatus(ActivityStatusCode.Error, "ChipId is required and cannot be empty");
            return BadRequest(new { Error = "ChipId is required and cannot be empty." });
        }

        if (chipId.Length > 50)
        {
            activity?.SetTag("validation.chipId", "too_long");
            activity?.SetStatus(ActivityStatusCode.Error, "ChipId must be 50 characters or less");
            return BadRequest(new { Error = "ChipId must be 50 characters or less." });
        }

        // Validate model state
        if (!ModelState.IsValid)
        {
            activity?.SetTag("validation.modelState", "invalid");
            activity?.SetStatus(ActivityStatusCode.Error, "Model validation failed");
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                );
            return BadRequest(new { Error = "Validation failed", Details = errors });
        }

        activity?.SetTag("validation.status", "passed");

        // Save sensor data to database
        try
        {
            using var dbActivity = ActivitySource.StartActivity("SensorController.SaveToDatabase");
            dbActivity?.SetTag("database.operation", "insert");
            dbActivity?.SetTag("sensor.chipId", chipId);
            
            // Add sensor data tags for monitoring
            if (sensorData != null)
            {
                dbActivity?.SetTag("sensor.wifi", sensorData.Wifi);
                dbActivity?.SetTag("sensor.rco2", sensorData.Rco2);
                dbActivity?.SetTag("sensor.pm02", sensorData.Pm02);
                dbActivity?.SetTag("sensor.atmp", sensorData.Atmp);
                dbActivity?.SetTag("sensor.rhum", sensorData.Rhum);

                await _context.SensorData.AddAsync(new SensorDatum
                {
                    ChipId = chipId,
                    Wifi = sensorData.Wifi,
                    Rco2 = sensorData.Rco2,
                    Pm02 = sensorData.Pm02,
                    Atmp = sensorData.Atmp,
                    Rhum = sensorData.Rhum,
                    Timestamp = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            dbActivity?.SetTag("database.status", "success");
            activity?.SetTag("operation.status", "success");
            _logger.LogInformation("Successfully saved sensor data for chipId: {ChipId}", chipId);
            return Ok(new { Message = "Sensor data received successfully." });
        }
        catch (DbUpdateException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Database update error");
            activity?.SetTag("error.type", nameof(DbUpdateException));
            _logger.LogError(ex, "Database update error while saving sensor data for chipId: {ChipId}", chipId);
            return StatusCode(500, new { Error = "Failed to save sensor data. Please try again later." });
        }
        catch (DbException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Database connection error");
            activity?.SetTag("error.type", nameof(DbException));
            _logger.LogError(ex, "Database connection error while saving sensor data for chipId: {ChipId}", chipId);
            return StatusCode(500, new { Error = "Database connection error. Please try again later." });
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unexpected error");
            activity?.SetTag("error.type", ex.GetType().Name);
            _logger.LogError(ex, "Unexpected error while saving sensor data for chipId: {ChipId}", chipId);
            return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
        }
    }
}