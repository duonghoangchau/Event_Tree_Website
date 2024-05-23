using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class DetailEvent
    {
        public int Id { get; set; }
        public int? EventId { get; set; }
        public int? TreeId { get; set; }

        public virtual Event? Event { get; set; }
        public virtual Tree? Tree { get; set; }
    }
}
