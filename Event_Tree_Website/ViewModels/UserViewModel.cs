using Event_Tree_Website.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Event_Tree_Website.ViewModels
{
    public class UserViewModel
    {
        public List<Menu> Menus { get; set; }
        public User Register { get; set; }
        public UserViewModel()
        {
            Register = new User();
        }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [StringLength(100, ErrorMessage = "Mật khẩu mới phải dài từ 6 đến 100 ký tự")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
