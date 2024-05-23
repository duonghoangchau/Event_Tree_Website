using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Menu
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? MenuOrder { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }
    }
}
