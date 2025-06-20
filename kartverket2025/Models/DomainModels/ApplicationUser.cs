using Microsoft.AspNetCore.Identity;

namespace kartverket2025.Models.DomainModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        

    }
}
