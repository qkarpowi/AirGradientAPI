using AirGradientAPI.Controllers;
using AirGradientAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AirGradientAPI.Tests.Controller;

public class SensorControllerTests : IDisposable
{
    private readonly DataContext _context;
    private readonly SensorController _controller;

    public SensorControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new DataContext(options);
        var mockLogger = new Mock<ILogger<SensorController>>();
        _controller = new SensorController(_context, mockLogger.Object);
    }

    [Fact]
    public async Task ReceiveSensorData_ValidData_ReturnsOk()
    {
        // Arrange
        var chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        
        var response = okResult.Value;
        Assert.NotNull(response);

        // Verify data was saved
        var savedData = await _context.SensorData.FirstOrDefaultAsync(x => x.ChipId == chipId);
        Assert.NotNull(savedData);
        Assert.Equal(chipId, savedData.ChipId);
        Assert.Equal(sensorData.Wifi, savedData.Wifi);
        Assert.Equal(sensorData.Rco2, savedData.Rco2);
        Assert.Equal(sensorData.Pm02, savedData.Pm02);
        Assert.Equal(sensorData.Atmp, savedData.Atmp);
        Assert.Equal(sensorData.Rhum, savedData.Rhum);
        Assert.NotNull(savedData.Timestamp);
    }

    [Fact]
    public async Task ReceiveSensorData_EmptyChipId_ReturnsBadRequest()
    {
        // Arrange
        var chipId = "";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_NullChipId_ReturnsBadRequest()
    {
        // Arrange
        string chipId = null!;
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_ChipIdTooLong_ReturnsBadRequest()
    {
        // Arrange
        var chipId = new string('A', 51); // 51 characters, exceeds 50 character limit
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_InvalidWifiRange_ReturnsBadRequest()
    {
        // Arrange
        var chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -150, // Invalid: below -100
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Simulate model validation
        _controller.ModelState.AddModelError("Wifi", "WiFi signal strength must be between -100 and 0 dBm");

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_InvalidCo2Range_ReturnsBadRequest()
    {
        // Arrange
        var chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 60000, // Invalid: above 50000
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Simulate model validation
        _controller.ModelState.AddModelError("Rco2", "CO2 reading must be between 0 and 50000 ppm");

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_InvalidPm02Range_ReturnsBadRequest()
    {
        // Arrange
        const string chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 1500, // Invalid: above 1000
            Atmp = 72.5f,
            Rhum = 45
        };

        // Simulate model validation
        _controller.ModelState.AddModelError("Pm02", "PM2.5 reading must be between 0 and 1000 µg/m³");

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_InvalidTemperatureRange_ReturnsBadRequest()
    {
        // Arrange
        var chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 200f, // Invalid: above 176
            Rhum = 45
        };

        // Simulate model validation
        _controller.ModelState.AddModelError("Atmp", "Temperature must be between -40 and 176°F");

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task ReceiveSensorData_InvalidHumidityRange_ReturnsBadRequest()
    {
        // Arrange
        var chipId = "TEST-CHIP-001";
        var sensorData = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 150 // Invalid: above 100
        };

        // Simulate model validation
        _controller.ModelState.AddModelError("Rhum", "Humidity must be between 0 and 100%");

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Theory]
    [InlineData(-100, 0, 0, -40f, 0)]     // Minimum valid values
    [InlineData(0, 50000, 1000, 176f, 100)] // Maximum valid values
    [InlineData(-75, 400, 25, 72.5f, 50)]   // Typical values
    public async Task ReceiveSensorData_BoundaryValues_ReturnsOk(int wifi, int rco2, int pm02, float atmp, int rhum)
    {
        // Arrange
        var chipId = $"TEST-CHIP-{Guid.NewGuid():N}";
        var sensorData = new SensorDataModel
        {
            Wifi = wifi,
            Rco2 = rco2,
            Pm02 = pm02,
            Atmp = atmp,
            Rhum = rhum
        };

        // Act
        var result = await _controller.ReceiveSensorData(chipId, sensorData);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        // Verify data was saved with correct values
        var savedData = await _context.SensorData.FirstOrDefaultAsync(x => x.ChipId == chipId);
        Assert.NotNull(savedData);
        Assert.Equal(wifi, savedData.Wifi);
        Assert.Equal(rco2, savedData.Rco2);
        Assert.Equal(pm02, savedData.Pm02);
        Assert.Equal(atmp, savedData.Atmp);
        Assert.Equal(rhum, savedData.Rhum);
    }

    [Fact]
    public async Task ReceiveSensorData_MultipleSensorReadings_AllSavedCorrectly()
    {
        // Arrange
        var readings = new[]
        {
            new { ChipId = "CHIP-001", Data = new SensorDataModel { Wifi = -50, Rco2 = 400, Pm02 = 15, Atmp = 72.5f, Rhum = 45 } },
            new { ChipId = "CHIP-002", Data = new SensorDataModel { Wifi = -60, Rco2 = 450, Pm02 = 20, Atmp = 73.0f, Rhum = 50 } },
            new { ChipId = "CHIP-003", Data = new SensorDataModel { Wifi = -70, Rco2 = 500, Pm02 = 25, Atmp = 74.5f, Rhum = 55 } }
        };

        // Act & Assert
        foreach (var reading in readings)
        {
            var result = await _controller.ReceiveSensorData(reading.ChipId, reading.Data);
            Assert.IsType<OkObjectResult>(result);
        }

        // Verify all readings were saved
        var savedCount = await _context.SensorData.CountAsync();
        Assert.Equal(readings.Length, savedCount);

        foreach (var reading in readings)
        {
            var savedData = await _context.SensorData.FirstOrDefaultAsync(x => x.ChipId == reading.ChipId);
            Assert.NotNull(savedData);
            Assert.Equal(reading.Data.Wifi, savedData.Wifi);
            Assert.Equal(reading.Data.Rco2, savedData.Rco2);
            Assert.Equal(reading.Data.Pm02, savedData.Pm02);
            Assert.Equal(reading.Data.Atmp, savedData.Atmp);
            Assert.Equal(reading.Data.Rhum, savedData.Rhum);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
