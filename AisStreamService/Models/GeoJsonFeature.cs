namespace AisStreamService.Models
{
    public class GeoJsonFeature
    {
        public string Type { get; set; } = "Feature";
        public Dictionary<string, object> Properties { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; } = "Point";
        public List<double> Coordinates { get; set; }
    }
}