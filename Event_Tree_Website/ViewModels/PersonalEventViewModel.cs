using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Tree_Website.ViewModels
{
    public class PersonalEventViewModel
    {
        internal string cateName;

        public List<Menu> Menus { get; set; }
        public List<PersonalEvent> Pers { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public PersonalEvent Personals { get; internal set; }
        public List<SelectListItem> HideOptions { get; internal set; }
        public List<Image> Images { get; internal set; }
    }
}