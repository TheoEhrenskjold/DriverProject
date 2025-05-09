using Microsoft.AspNetCore.Identity;

namespace Gather.Models
{
    public class User : IdentityUser
    {
        //public Guid Id { get; set; }
        public string Name { get; set; }
        //public string Email { get; set; }
        //public string Description { get; set; }
        //public string PasswordHash { get; set; }
    }
}
