﻿using Event_Tree_Website.Models;

namespace Event_Tree_Website.ViewModels
{
    internal class EventViewModel
    {
        internal string cateName;

        public List<Menu> Menus { get; set; }
        public List<Event> Eves { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}