using System;
namespace reservation_fc_ms.Models
{
	public class ReservationOption
	{
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int ReservationId { get; set; }
        public int OptionId { get; set; } = 0!;

        public ReservationOption()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}

