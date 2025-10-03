using System;
using System.Collections.Generic;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MGE_HEROES.Server
{
    [Table("users")]
    public class User : BaseModel
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("id")]
        public Guid Id { get; set; }

        [Column("password")]
        public string PasswordHash { get; set; }

        [Column("email")]
        public string EmailHash { get; set; }
        public User()
        {
            Username = "quest";
        }

    }
}
