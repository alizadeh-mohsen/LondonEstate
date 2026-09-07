namespace LondonEstate.Core.Dtos
{
    public class ReminderDto
    {
        public Guid Id { get; set; }
        public DateOnly Due { get; set; }
        public string? Note { get; set; }
    }
}
