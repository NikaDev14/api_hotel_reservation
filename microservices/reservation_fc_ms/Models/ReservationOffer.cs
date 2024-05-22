using System;
namespace reservation_fc_ms.Models
{
	public class ReservationOffer
	{
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int OfferId { get; set; }
    }
}

