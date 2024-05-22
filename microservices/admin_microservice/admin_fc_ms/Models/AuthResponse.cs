using System;
namespace admin_fc_ms.Models
{
	public class AuthResponse
	{
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}

