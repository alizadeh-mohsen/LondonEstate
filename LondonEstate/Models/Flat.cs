namespace LondonEstate.Models
{
    public class Flat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool Empty { get; set; } = true;
        public string SSID { get; set; }
        public string Password { get; set; }
        public string CheckinInstruction { get; set; }
        public string VisualGuideUrl { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }


    }
}
