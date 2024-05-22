using System;
namespace UserFcApi.Models
{
	public class AuthRequest
	{
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

