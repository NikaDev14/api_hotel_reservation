using System;
using options_fc_ms.Models;
using Microsoft.EntityFrameworkCore;

namespace options_fc_ms.Data
{
	public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Option> Options { get; set; }
        public DbSet<HotelOption> HotelOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}

