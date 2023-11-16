using Microsoft.AspNetCore.Mvc;
using AirGradientAPI.Models;
using AirGradientAPI.Entities;

namespace AirGradientAPI.Controllers;

[ApiController]
[Route("api/v1/sensors")]
public class SensorController : ControllerBase
{
    private readonly DataContext _context;
    public SensorController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("airgradient:{chipId}/measures")]
    public IActionResult ReceiveSensorData(string chipId, [FromBody] SensorDataModel sensorData)
    {
        // Do something with the received sensor data
        // For example, we will use efcore to save it to the database
        _context.SensorData.Add(new SensorDatum
        {
            ChipId = chipId,
            Wifi = sensorData.Wifi,
            Rco2 = sensorData.Rco2,
            Pm02 = sensorData.Pm02,
            Atmp = sensorData.Atmp,
            Rhum = sensorData.Rhum,
            timestamp = DateTime.Now
        });
        _context.SaveChanges();

        // Return a success response
        return Ok(new { Message = "Sensor data received successfully." });
    }
}