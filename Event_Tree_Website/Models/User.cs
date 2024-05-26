using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Contributions = new HashSet<Contribution>();
            Orders = new HashSet<Order>();
            Trees = new HashSet<Tree>();
        }

        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Gender { get; set; }
        public int? Provide { get; set; }
        public int? Role { get; set; }
        public int? PremiumId { get; set; }
        public DateTime? PremiumDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? Status { get; set; }
        public string? Avatar { get; set; }

        public virtual Premium? Premium { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Contribution> Contributions { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Tree> Trees { get; set; }
    }
}
