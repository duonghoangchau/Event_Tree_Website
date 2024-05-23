using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Comment
    {
        public long Id { get; set; }
        public string CmtContent { get; set; } = null!;
        public int? UserId { get; set; }
        public int? EventId { get; set; }
        public bool? Active { get; set; }
        public long? Likes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Event? Event { get; set; }
        public virtual User? User { get; set; }
    }
}
