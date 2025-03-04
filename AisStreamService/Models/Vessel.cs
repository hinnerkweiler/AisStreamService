namespace AisStreamService.Models;

public class Vessel
{
    public int Id { get; set; }
    public int Mmsi { get; set; }
    public string ShipName { get; set; }
    public double Speed { get; set; }
    public double Course { get; set; }
    public double Latitude { get; set; } = 53.8593;
    public double Longitude { get; set; } = 8.6879;
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public string? ShipUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? Country { get; set; }
    public string? ShipType { get; set; }
    public string GroupId { get; set; } = Environment.GetEnvironmentVariable("Group") ?? "ungrouped";
}
