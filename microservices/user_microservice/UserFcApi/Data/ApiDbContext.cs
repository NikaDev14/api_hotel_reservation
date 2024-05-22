using System;
using Microsoft.EntityFrameworkCore;
using UserFcApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace UserFcApi.Data
{
    public class APIDbContext : IdentityUserContext<IdentityUser>
    {

        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // It would be a good idea to move the connection string to user secrets
            options.UseNpgsql("Host=postgres;Database=SampleDbDriver;Username=postgres;Password=postgres");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}