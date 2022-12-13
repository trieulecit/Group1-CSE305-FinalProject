using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DeliveryAgentManagementApp.Models;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DeliveryAgentManagementApp.Data
{
    public class DeliveryAgentManagementAppContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DeliveryAgentManagementAppContext (DbContextOptions<DeliveryAgentManagementAppContext> options)
            : base(options)
        {
        }

        public DbSet<DeliveryAgentManagementApp.Models.Order> Order { get; set; } = default!;

        public DbSet<DeliveryAgentManagementApp.Models.Courier> Courier { get; set; }
        
        public DbSet<DeliveryAgentManagementApp.Models.Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*           modelBuilder.Entity<Account>()
                          .HasIndex(a => a.Phone)
                          .IsUnique();*/
            modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(x => new { x.UserId, x.LoginProvider });
            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(x => new { x.UserId, x.LoginProvider });


        }

        
    }

    
}
