using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Contribution
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? EventName { get; set; }
        public DateTime? DateTime { get; set; }
        public string? ImageCode { get; set; }
        public bool? Status { get; set; }
        public bool? Approval { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
