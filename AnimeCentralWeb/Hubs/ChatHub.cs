using AnimeCentralWeb.Data;
using AnimeCentralWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace AnimeCentralWeb.Hubs
{
    [HubName("Chat")]
    public class ChatHub : Hub
    {
        private UserManager<ApplicationUser> UserManager;
        private IHttpContextAccessor HttpContextAccessor;
        public ChatHub(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            UserManager = userManager;
            HttpContextAccessor = httpContextAccessor;
        }
        
        public async void send(string message)
        {
            var user = await UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User);
            if (user != null)
                Clients.All.broadcastMessage(new { username = user.UserName, image = user.Image != null ? user.Image.ToString() : "images/DefaultAvatar.jpg" }, new { content = message, date = DateTime.Now.ToString("d/M/yyyy hh:mm")});
        }
    }
}
