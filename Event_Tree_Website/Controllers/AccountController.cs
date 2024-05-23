using Microsoft.AspNetCore.Mvc;
using Event_Tree_Website.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class AccountController : Controller
    {
        Event_TreeContext db = new Event_TreeContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                /*var u = db.Users.Where(x => x.Account.Username.Equals(user.Account.Username) &&
                    x.Account.Password.Equals(user.Account.Password)).FirstOrDefault();*/
                var u = db.Users.Include(x => x.Account).Where(x => x.Account.Username.Equals(user.Account.Username) &&
                x.Account.Password.Equals(user.Account.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("Username", u.Account.Username.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
    }
}
