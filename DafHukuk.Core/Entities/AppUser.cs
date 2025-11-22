using Microsoft.AspNetCore.Identity;

namespace DafHukuk.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } 
    }
}