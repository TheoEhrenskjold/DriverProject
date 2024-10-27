using System.ComponentModel.DataAnnotations;

namespace DriverProject.Models
{
    public class Driver
    {
        public Guid DriverID { get; set; }        
        public string DriverName { get; set; }
        public string CarReg { get; set; }

        // Relation till event
        public List<Event> Events { get; set; }

    }
}
