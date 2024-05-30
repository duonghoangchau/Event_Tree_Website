using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Tree_Website.ViewModels
{
    public class AccountViewModel
    {
        public User User { get; set; }
        public List<User> Users { get; set; }
        public List<Menu> Menus { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<SelectListItem> HideOptions { get; set; }
        public List<SelectListItem> PerOptions { get; set; }

    }
}
