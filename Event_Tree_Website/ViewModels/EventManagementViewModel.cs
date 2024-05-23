using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Tree_Website.ViewModels
{
    internal class EventManagementViewModel
    {
        internal string cateName;

        public List<Menu> Menus { get; set; }
        public Event Events { get; set; }
        public List<Event> Eves { get; internal set; }
        public int TotalPages { get; internal set; }
        public int CurrentPage { get; internal set; }
        public List<SelectListItem> HideOptions { get; internal set; }
    }
}