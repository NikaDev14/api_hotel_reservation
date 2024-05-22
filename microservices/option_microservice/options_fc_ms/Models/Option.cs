using System;
namespace options_fc_ms.Models
{
	public class Option
	{
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public String? UserUid { get; set; } = null!;
    }
}

