using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace AirGradientAPI.Models;

[SwaggerSchema("Sensor data measurements from an AirGradient device")]
public class SensorDataModel
{
    [Range(-100, 0, ErrorMessage = "WiFi signal strength must be between -100 and 0 dBm")]
    [SwaggerSchema("WiFi signal strength in dBm (typically -100 to 0)")]
    public int Wifi { get; set; }

    [Range(0, 50000, ErrorMessage = "CO2 reading must be between 0 and 50000 ppm")]
    [SwaggerSchema("CO2 concentration in parts per million (ppm)")]
    public int Rco2 { get; set; }

    [Range(0, 1000, ErrorMessage = "PM2.5 reading must be between 0 and 1000 µg/m³")]
    [SwaggerSchema("PM2.5 particle concentration in micrograms per cubic meter")]
    public int Pm02 { get; set; }

    [Range(-40, 176, ErrorMessage = "Temperature must be between -40 and 176°F")]
    [SwaggerSchema("Ambient temperature in Fahrenheit")]
    public float Atmp { get; set; }

    [Range(0, 100, ErrorMessage = "Humidity must be between 0 and 100%")]
    [SwaggerSchema("Relative humidity percentage")]
    public int Rhum { get; set; }
}