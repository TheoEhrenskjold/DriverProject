namespace Gather.Models
{
    public class Applications
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Events Event { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool Status { get; set; } = false;
    }
}
