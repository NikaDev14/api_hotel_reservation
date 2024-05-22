using System;
namespace hotel_fc_ms.Models
{
	public class RoomHotel
	{
        public int Id { get; set; }
        public int RoomId { get; set; } = 0!;
        public int HotelId { get; set; } = 0!;
        public int nbItems { get; set; } = 0!;
        public String? UserUid { get; set; } = null!;
    }
}

