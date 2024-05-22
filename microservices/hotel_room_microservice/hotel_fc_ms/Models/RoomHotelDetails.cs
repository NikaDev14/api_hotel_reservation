using System;
namespace hotel_fc_ms.Models
{
	public class RoomHotelDetails
	{
        public string Name { get; set; } = null!;
        public string NameShortcut { get; set; } = null!;
        public int nbPersonsMax { get; set; } = 0!;
        public float Price { get; set; } = 0.0F!;
        public int RemainingRoom { get; set; } = 0!;
    }
}

