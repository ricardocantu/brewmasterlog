using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BrewMasterWebApp.Models
{
    public class BrewMasterUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
