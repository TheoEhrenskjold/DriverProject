using System;
using System.ComponentModel.DataAnnotations;

namespace DriverProject.Models
{
    public class Event
    {
        public int EventID { get; set; }
        [Required(ErrorMessage = "Cannot be empty")]        
        public DateTime NoteDate { get; set; }
        [Required(ErrorMessage = "Cannot be empty")]
        public string NoteDescription { get; set; }
        [Required(ErrorMessage = "Cannot be empty")]
        public decimal BeloppIn { get; set; }
        [Required(ErrorMessage = "Cannot be empty")]
        public decimal BeloppUt { get; set; }

        // Relation till förare
        public Guid DriverID { get; set; }
        public Driver Driver { get; set; }
    }


}
