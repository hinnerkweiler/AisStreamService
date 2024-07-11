namespace AisStreamService.Models
{
    public class AisStreamResponse
    {
        public string MessageType { get; set; }
        public PositionReport Message { get; set; }
    }

    public class PositionReport
    {
        public int UserID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ShipName { get; set; }
    }
}