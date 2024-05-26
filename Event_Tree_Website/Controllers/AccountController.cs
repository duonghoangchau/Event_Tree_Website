using Microsoft.AspNetCore.Mvc;
using Event_Tree_Website.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Event_Tree_Website.ViewModels;

namespace Event_Tree_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly Event_TreeContext db;
        public AccountController(Event_TreeContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m =>
           m.MenuOrder).ToListAsync();
            var viewModel = new UserViewModel
            {
                Menus = menus
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new UserViewModel
            {
                Menus = menus,

            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m =>
           m.MenuOrder).ToListAsync();

            var viewModel = new UserViewModel
            {
                Menus = menus,

                Register = model.Register,
            };
            if (model.Register != null)
            {
                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username ==
               model.Register.Username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại.";
                    return View(viewModel);
                }
                model.Register.Password =
               BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                model.Register.Role = 0;
                model.Register.Status = true;
                db.Users.Add(model.Register);
                await db.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m =>
           m.MenuOrder).ToListAsync();

            var viewModel = new UserViewModel
            {
                Menus = menus,

                Register = model.Register,
            };
            if (model.Register != null)
            {
                var user = await db.Users.Where(m => m.Status == true).FirstOrDefaultAsync(u => u.Username ==
               model.Register.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Register.Password,
               user.Password))
                {
                    var claims = new List<Claim>
                     {
                     new Claim(ClaimTypes.Name, user.Username),
                   new Claim(ClaimTypes.Role, user.Role.ToString()),
              };
                    var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                    };
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Info()
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m =>
           m.MenuOrder).ToListAsync();
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = await db.Users.FirstOrDefaultAsync(m => m.Username ==
                   username);
                }
            }
            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = users,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
