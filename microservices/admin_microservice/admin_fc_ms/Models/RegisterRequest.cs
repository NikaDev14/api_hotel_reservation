using System;
using System.ComponentModel.DataAnnotations;

namespace admin_fc_ms.Models
{
	public class RegisterRequest
	{
        /*
		public RegisterRequest()
		{
		}
        */
        public RegisterRequest()
        {
            this.Role = UserRole.ROLE_ADMIN;
        }
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public UserRole Role { get; set; }
        [Required]
        public string Password { get; set; } = null!;
    }
}

