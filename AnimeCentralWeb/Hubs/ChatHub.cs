using AnimeCentralWeb.Data;
using AnimeCentralWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System.Threading.Tasks;

namespace AnimeCentralWeb.Hubs
{
    [HubName("Chat")]
    public class ChatHub : Hub
    {
        private UserManager<ApplicationUser> UserManager;
        private IHttpContextAccessor HttpContextAccessor;
        private AnimeCentralDbContext Context;
        public ChatHub(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, AnimeCentralDbContext context)
        {
            UserManager = userManager;
            HttpContextAccessor = httpContextAccessor;
            Context = context;
        }
        
        public async void join() {
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User);
            if(user != null)
                Clients.All.broadcastMessage(new { username = user.UserName, image = user.Image}, $"{user.UserName} s-a logat.");
        }

        public async Task send(string message)
        {
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User);

            if (user != null)
                Clients.All.broadcastMessage(new { username = user.UserName, image = user.Image }, message);
        }
    }
}
