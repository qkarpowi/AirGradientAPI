using System.ComponentModel.DataAnnotations;
using AirGradientAPI.Models;

namespace AirGradientAPI.Tests.Models;

public class SensorDataModelTests
{
    [Theory]
    [InlineData(-100, true)]  // Min valid wifi
    [InlineData(-50, true)]   // Typical wifi
    [InlineData(0, true)]     // Max valid wifi
    [InlineData(-101, false)] // Below min
    [InlineData(1, false)]    // Above max
    public void Wifi_Validation_WorksCorrectly(int wifi, bool isValid)
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = wifi,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var validationResults = ValidateModel(model);
        var wifiErrors = validationResults.Where(v => v.MemberNames.Contains(nameof(model.Wifi)));

        // Assert
        if (isValid)
        {
            Assert.Empty(wifiErrors);
        }
        else
        {
            Assert.NotEmpty(wifiErrors);
            Assert.Contains("WiFi signal strength must be between -100 and 0 dBm", 
                wifiErrors.First().ErrorMessage);
        }
    }

    [Theory]
    [InlineData(0, true)]      // Min valid CO2
    [InlineData(400, true)]    // Typical CO2
    [InlineData(50000, true)]  // Max valid CO2
    [InlineData(-1, false)]    // Below min
    [InlineData(50001, false)] // Above max
    public void Rco2_Validation_WorksCorrectly(int rco2, bool isValid)
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = rco2,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var validationResults = ValidateModel(model);
        var rco2Errors = validationResults.Where(v => v.MemberNames.Contains(nameof(model.Rco2)));

        // Assert
        if (isValid)
        {
            Assert.Empty(rco2Errors);
        }
        else
        {
            Assert.NotEmpty(rco2Errors);
            Assert.Contains("CO2 reading must be between 0 and 50000 ppm", 
                rco2Errors.First().ErrorMessage);
        }
    }

    [Theory]
    [InlineData(0, true)]     // Min valid PM2.5
    [InlineData(15, true)]    // Typical PM2.5
    [InlineData(1000, true)]  // Max valid PM2.5
    [InlineData(-1, false)]   // Below min
    [InlineData(1001, false)] // Above max
    public void Pm02_Validation_WorksCorrectly(int pm02, bool isValid)
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = pm02,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var validationResults = ValidateModel(model);
        var pm02Errors = validationResults.Where(v => v.MemberNames.Contains(nameof(model.Pm02)));

        // Assert
        if (isValid)
        {
            Assert.Empty(pm02Errors);
        }
        else
        {
            Assert.NotEmpty(pm02Errors);
            Assert.Contains("PM2.5 reading must be between 0 and 1000 µg/m³", 
                pm02Errors.First().ErrorMessage);
        }
    }

    [Theory]
    [InlineData(-40f, true)]   // Min valid temperature
    [InlineData(72.5f, true)]  // Typical temperature
    [InlineData(176f, true)]   // Max valid temperature
    [InlineData(-41f, false)]  // Below min
    [InlineData(177f, false)]  // Above max
    public void Atmp_Validation_WorksCorrectly(float atmp, bool isValid)
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = atmp,
            Rhum = 45
        };

        // Act
        var validationResults = ValidateModel(model);
        var atmpErrors = validationResults.Where(v => v.MemberNames.Contains(nameof(model.Atmp)));

        // Assert
        if (isValid)
        {
            Assert.Empty(atmpErrors);
        }
        else
        {
            Assert.NotEmpty(atmpErrors);
            Assert.Contains("Temperature must be between -40 and 176°F", 
                atmpErrors.First().ErrorMessage);
        }
    }

    [Theory]
    [InlineData(0, true)]    // Min valid humidity
    [InlineData(45, true)]   // Typical humidity
    [InlineData(100, true)]  // Max valid humidity
    [InlineData(-1, false)]  // Below min
    [InlineData(101, false)] // Above max
    public void Rhum_Validation_WorksCorrectly(int rhum, bool isValid)
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = rhum
        };

        // Act
        var validationResults = ValidateModel(model);
        var rhumErrors = validationResults.Where(v => v.MemberNames.Contains(nameof(model.Rhum)));

        // Assert
        if (isValid)
        {
            Assert.Empty(rhumErrors);
        }
        else
        {
            Assert.NotEmpty(rhumErrors);
            Assert.Contains("Humidity must be between 0 and 100%", 
                rhumErrors.First().ErrorMessage);
        }
    }

    [Fact]
    public void ValidModel_PassesAllValidation()
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -50,
            Rco2 = 400,
            Pm02 = 15,
            Atmp = 72.5f,
            Rhum = 45
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void InvalidModel_FailsMultipleValidations()
    {
        // Arrange
        var model = new SensorDataModel
        {
            Wifi = -200,    // Invalid
            Rco2 = 60000,   // Invalid
            Pm02 = 1500,    // Invalid
            Atmp = 200f,    // Invalid
            Rhum = 150      // Invalid
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Equal(5, validationResults.Count); // All 5 properties should have errors
        
        Assert.Contains(validationResults, v => 
            v.MemberNames.Contains(nameof(model.Wifi)) && 
            v.ErrorMessage!.Contains("WiFi signal strength"));
        
        Assert.Contains(validationResults, v => 
            v.MemberNames.Contains(nameof(model.Rco2)) && 
            v.ErrorMessage!.Contains("CO2 reading"));
        
        Assert.Contains(validationResults, v => 
            v.MemberNames.Contains(nameof(model.Pm02)) && 
            v.ErrorMessage!.Contains("PM2.5 reading"));
        
        Assert.Contains(validationResults, v => 
            v.MemberNames.Contains(nameof(model.Atmp)) && 
            v.ErrorMessage!.Contains("Temperature"));
        
        Assert.Contains(validationResults, v => 
            v.MemberNames.Contains(nameof(model.Rhum)) && 
            v.ErrorMessage!.Contains("Humidity"));
    }

    private static IList<ValidationResult> ValidateModel(SensorDataModel model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}