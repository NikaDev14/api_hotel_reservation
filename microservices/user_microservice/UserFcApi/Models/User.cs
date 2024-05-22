using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace UserFcApi.Models
{
    public class User
	{

        public User(string username, string password, UserRole role= UserRole.ROLE_USER)//, string password, UserRole role = UserRole.ROLE_USER
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public UserRole Role { get; set; }
    }
}

