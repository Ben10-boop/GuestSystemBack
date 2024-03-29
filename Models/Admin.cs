﻿using System.Text.Json.Serialization;

namespace GuestSystemBack.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        [JsonIgnore]
        public byte[] PasswordHash { get; set; } = null;
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; } = null;
        public string Role { get; set; } = "regular"; //regular, super, removed
    }
}
