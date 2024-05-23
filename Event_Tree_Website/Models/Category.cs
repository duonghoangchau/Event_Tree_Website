using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Category
    {
        public Category()
        {
            Events = new HashSet<Event>();
        }

        public int IdCategory { get; set; }
        public string? Name { get; set; }
        public int? Order { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }
        public string? IdParent { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
