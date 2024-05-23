using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Event
    {
        public Event()
        {
            Comments = new HashSet<Comment>();
            DetailEvents = new HashSet<DetailEvent>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? ImageCode { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }
        public string? Status { get; set; }
        public int? IdCategory { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Category? IdCategoryNavigation { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DetailEvent> DetailEvents { get; set; }
    }
}
