using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;


namespace admin_fc_ms.Data
{
    public class APIDbContext : IdentityUserContext<IdentityUser>
    {

        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=admin_pg;Database=AdminDbDriver;Username=postgres;Password=postgres");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}