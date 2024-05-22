using System;
namespace reservation_fc_ms.Models
{
	public class Offer
	{
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Day { get; set; } = null;
        public int Percentage { get; set; }
    }
}

