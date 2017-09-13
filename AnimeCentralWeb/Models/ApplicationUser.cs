using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AnimeCentralWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Image { get; set; }
        public string NotificationTokens { get; set; }
        public string Status { get; set; }
        public string Bio { get; set; }

    }
}
