using Microsoft.AspNetCore.Identity;

namespace Vasko.Data
{
    public class ApplicationUser : IdentityUser
    {
        public byte[]? Avatar { get; set; }
    }
}