using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Image
    {
        public long Id { get; set; }
        public string ImageCode { get; set; } = null!;
        public string? Url { get; set; }
        public int? Type { get; set; }
    }
}
