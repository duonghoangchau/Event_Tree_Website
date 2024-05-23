using System;
using System.Collections.Generic;

namespace Event_Tree_Website.Models
{
    public partial class Slider
    {
        public int IdSlide { get; set; }
        public string? Title { get; set; }
        public string? Img { get; set; }
        public int? Order { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }
    }
}
