using System.ComponentModel.DataAnnotations;

namespace Gather.Models
{
    public class Events
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        [Required(ErrorMessage = "Vänligen fyll i ett värde!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vänligen fyll i ett värde!")]
        public DateTime CreatedDate { get; set; }        
        public DateTime EventDate { get; set; }
        [Required(ErrorMessage = "Vänligen välj ett datum!")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "Vänligen välj ett datum!")]
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublic{ get; set; }
        public string Address{ get; set; }
        [Required(ErrorMessage = "Välj en adress!")]
        public Guid HostUserId { get; set; }
        public User HostUser { get; set; }
    }
}
