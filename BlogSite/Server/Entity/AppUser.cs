using Microsoft.AspNetCore.Identity;

namespace BlogSite.Server.Entity
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
