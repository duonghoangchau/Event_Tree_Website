using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Premium
    {
        public Premium()
        {
            Orders = new HashSet<Order>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? PremiumLevel { get; set; }
        public long? Price { get; set; }
        public string? Description { get; set; }
        public int? ValidityPeriod { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
