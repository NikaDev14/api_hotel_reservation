using System;
using System.ComponentModel.DataAnnotations;

namespace UserFcApi.Models
{
	public class RegisterRequest
	{
        /*
		public RegisterRequest()
		{
		}
        */
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

