using System;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;

namespace hotel_fc_ms.Data
{
	public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomHotel> RoomHotels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=room_hotel_pg;Database=room_hotel_db_driver;Username=postgres;Password=postgres");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}

