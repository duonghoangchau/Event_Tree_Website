﻿using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class PersonalEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? ImageCode { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? IdUser { get; set; }

        public virtual User? IdUserNavigation { get; set; }
    }
}
