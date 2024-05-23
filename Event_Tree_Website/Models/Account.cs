using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Account
    {
        public Account()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Provide { get; set; }
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? Info { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
