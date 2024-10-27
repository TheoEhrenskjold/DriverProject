using Microsoft.AspNetCore.Identity;

namespace DriverProject.Models
{
    public class Employee : IdentityUser
    {       
        public string Name { get; set; }        
        
    }
}
