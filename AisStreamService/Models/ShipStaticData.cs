namespace AisStreamService.Models;

public class Dimension
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
}

public class Eta
{
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Month { get; set; }
}

public class ShipStaticData
{
    public int AisVersion { get; set; }
    public string CallSign { get; set; }
    public string Destination { get; set; }
    public Dimension Dimension { get; set; }
    public bool Dte { get; set; }
    public Eta Eta { get; set; }
    public int FixType { get; set; }
    public int ImoNumber { get; set; }
    public double MaximumStaticDraught { get; set; }
    public int MessageID { get; set; }
    public string Name { get; set; }
    public int RepeatIndicator { get; set; }
    public bool Spare { get; set; }
    public int Type { get; set; }
    public int UserID { get; set; }
    public bool Valid { get; set; }
}
