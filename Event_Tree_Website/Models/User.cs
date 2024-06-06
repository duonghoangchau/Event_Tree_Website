using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class User
    {
        public User()
        {
            PersonalEvents = new HashSet<PersonalEvent>();
        }

        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<PersonalEvent> PersonalEvents { get; set; }
    }
}
