using System;
namespace admin_fc_ms.Models
{
	public class AuthRequest
	{
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

