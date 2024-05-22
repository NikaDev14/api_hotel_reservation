using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_fc_ms.Models
{
	public class Room
	{
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string NameShortcut { get;set; } = null!;
        public int nbPersonsMax { get;set; } = 0!;
        public float Price { get;set; } = 0.0F!;
        public String? UserUid { get; set; } = null!;
    }
}

