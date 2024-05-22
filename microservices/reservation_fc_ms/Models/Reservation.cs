using System;
namespace reservation_fc_ms.Models
{
	public class Reservation
	{

        public int Id { get; set; }
        public DateTime CreationDate  { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String? UserUid { get; set; }
        public int? RoomId { get; set; }
        public float totalAmount { get; set; }
        public bool? isValidated { get; set; }
        public Reservation()
		{
            CreationDate = DateTime.UtcNow;
		}
	}
}

