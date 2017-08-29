using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AnimeCentralWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public byte[] Image { get; set; }
        public string NotificationTokens { get; set; }

    }
}
