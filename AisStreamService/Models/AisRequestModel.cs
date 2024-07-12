namespace AisStreamService.Models
{
    public class AisRequestModel
    {
        public List<int>? MmsiNumbers { get; set; }
        public string? ShipName { get; set; }
        public string? Group { get; set; }
        public string ApiKey { get; set; }
    }
}