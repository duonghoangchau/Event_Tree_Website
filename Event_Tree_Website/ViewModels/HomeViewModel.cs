using Event_Tree_Website.Models;

namespace Event_Tree_Website.ViewModels
{
    public class HomeViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<Slider> Sliders { get; set; }
        public List<Category> CategoriesParent { get; set; }
        public List<Category> Categories { get; set; }
        public List<Event> Even { get; set; }
    }
}