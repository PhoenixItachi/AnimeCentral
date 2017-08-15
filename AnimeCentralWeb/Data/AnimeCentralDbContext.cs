using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Domain;

namespace AnimeCentralWeb.Data
{
    public class AnimeCentralDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Anime> Anime { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Source> Sources { get; set; }


        public AnimeCentralDbContext(DbContextOptions<AnimeCentralDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
