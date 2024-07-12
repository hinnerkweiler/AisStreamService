namespace AisStreamService.Models
{
    public class AisStreamResponse
    {
        public string MessageType { get; set; }
        public MessageContent Message { get; set; }
        public MetaData MetaData { get; set; }
    }

    public class MessageContent
    {
        public PositionReportClassB? StandardClassBPositionReport { get; set; }
        public PositionReport? PositionReport { get; set; }
        public ShipStaticData? ShipStaticData { get; set; }
    }

    public class MetaData
    {
        public int MMSI { get; set; }
       // public string MMSI_String { get; set; }
        public string ShipName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string time_utc { get; set; }
    }
}