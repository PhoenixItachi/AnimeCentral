using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Image { get; set; }
        public string NotificationTokens { get; set; }
        [Display(Prompt = "Status")]
        public string Status { get; set; }
        [Display(Prompt = "Despre tine")]
        public string Bio { get; set; }

    }
}
