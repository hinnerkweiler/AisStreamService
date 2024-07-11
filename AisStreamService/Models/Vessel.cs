namespace AisStreamService.Models;

public class Vessel
{
    public int Id { get; set; }
    public int Mmsi { get; set; }
    public string ShipName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime LastUpdated { get; set; }
}
