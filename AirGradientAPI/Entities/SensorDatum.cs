namespace AirGradientAPI.Entities;

public class SensorDatum
{
    public long Id { get; set; }
    public string ChipId { get; set; } = null!;
    public int Wifi { get; set; }
    public int Rco2 { get; set; }
    public int Pm02 { get; set; }
    public float Atmp { get; set; }
    public int Rhum { get; set; }
    public DateTime? Timestamp { get; set; }
}