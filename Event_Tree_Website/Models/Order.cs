using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Order
    {
        public int Id { get; set; }
        public long? Total { get; set; }
        public int? TypePayment { get; set; }
        public string? OrderCode { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? PremiumId { get; set; }

        public virtual Premium? Premium { get; set; }
        public virtual User? User { get; set; }
    }
}
