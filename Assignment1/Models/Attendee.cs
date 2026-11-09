namespace Assignment1.Models
{
    public class Attendee
    {
        public string Id { get; set; }
        public string Name {  get; set; }

        public string Email { get; set; }
        int EventId { get; set; }
        public Event Event { get; set; }
    }
}
