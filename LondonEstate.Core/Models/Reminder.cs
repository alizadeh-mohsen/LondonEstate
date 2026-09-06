namespace LondonEstate.Core.Models
{
    public class Reminder
    {
        public Guid Id { get; set; } = new Guid();
        public DateTime Due { get; set; }
        public string? Note { get; set; }
    }
}
