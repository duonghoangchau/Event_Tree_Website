using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Tree
    {
        public Tree()
        {
            DetailEvents = new HashSet<DetailEvent>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Active { get; set; }
        public string? ImageCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<DetailEvent> DetailEvents { get; set; }
    }
}
