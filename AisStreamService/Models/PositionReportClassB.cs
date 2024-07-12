namespace AisStreamService.Models;

public class PositionReportClassB
{
    public bool AssignedMode { get; set; }
    public bool ClassBBand { get; set; }
    public bool ClassBDisplay { get; set; }
    public bool ClassBDsc { get; set; }
    public bool ClassBMsg22 { get; set; }
    public bool ClassBUnit { get; set; }
    public double Cog { get; set; }
    public int CommunicationState { get; set; }
    public bool CommunicationStateIsItdma { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int MessageID { get; set; }
    public bool PositionAccuracy { get; set; }
    public bool Raim { get; set; }
    public int RepeatIndicator { get; set; }
    public double Sog { get; set; }
    public int Spare1 { get; set; }
    public int Spare2 { get; set; }
    public int Timestamp { get; set; }
    public int TrueHeading { get; set; }
    public int UserID { get; set; }
    public bool Valid { get; set; }
}