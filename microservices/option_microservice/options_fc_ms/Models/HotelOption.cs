using System;
namespace options_fc_ms.Models
{
	public class HotelOption
	{
        public int Id { get; set; }
        public int HotelId { get; set; } = 0!;
        public int OptionId { get; set; } = 0!;
        public int Quantity { get; set; } = 0!;
        public String? UserUid { get; set; } = null!;
    }
}

