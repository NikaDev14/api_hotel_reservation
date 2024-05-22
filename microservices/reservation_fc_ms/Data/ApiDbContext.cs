using System;
using reservation_fc_ms.Models;
using Microsoft.EntityFrameworkCore;

namespace reservation_fc_ms.Data
{
	public class ApiDbContext : DbContext
	{
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<ReservationOffer> ReservationOffers { get; set; }
        public DbSet<ReservationOption> ReservationOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}

