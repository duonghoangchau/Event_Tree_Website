using Microsoft.AspNetCore.Mvc;
using Event_Tree_Website.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;

namespace Event_Tree_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly Event_TreeContext _db;
        public AccountController(Event_TreeContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var menus = await GetMenusAsync();
            var viewModel = new UserViewModel { Menus = menus };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var menus = await GetMenusAsync();
            var viewModel = new UserViewModel { Menus = menus };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            var menus = await GetMenusAsync();
            var viewModel = new UserViewModel { Menus = menus, Register = model.Register };

            if (model.Register != null)
            {
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Register.Username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại.";
                    return View(viewModel);
                }

                model.Register.Password = BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                model.Register.Role = 0;
                model.Register.Status = true;

                _db.Users.Add(model.Register);
                await _db.SaveChangesAsync();

                return RedirectToAction("Login", "Account");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            var menus = await GetMenusAsync();
            var viewModel = new UserViewModel { Menus = menus, Register = model.Register };

            if (model.Register != null)
            {
                var user = await _db.Users.Where(m => m.Status == true).FirstOrDefaultAsync(u => u.Username == model.Register.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Register.Password, user.Password))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
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

        [HttpGet]
        public async Task<IActionResult> GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return RedirectToAction(nameof(Login));

            var claims = authenticateResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new { claim.Type, claim.Value }).ToList();
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (email != null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Username = email,
                        Fullname = name,
                        Email = email,
                        Role = 0,
                        Status = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("RandomPassword")
                    };
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                }

                await SignInUser(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> FacebookLogin()
        {
            var redirectUrl = Url.Action("FacebookResponse", "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return RedirectToAction(nameof(Login));

            var claims = authenticateResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new { claim.Type, claim.Value }).ToList();
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (email != null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Username = email,
                        Fullname = name,
                        Email = email,
                        Role = 0,
                        Status = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("RandomPassword")
                    };
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                }

                await SignInUser(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Info()
        {
            var menus = await _db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var users = new User();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    users = await _db.Users.FirstOrDefaultAsync(m => m.Username == username);
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


        [HttpGet]
        public async Task<IActionResult> EditInfo()
        {
            var menus = await _db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = currentUser
            };
            return View(viewModel);
        }

        private async Task<User> GetCurrentUserAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                if (username != null)
                {
                    return await _db.Users.FirstOrDefaultAsync(m => m.Username == username);
                }
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveInfo(UserViewModel model)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                currentUser.Fullname = model.Register.Fullname;
                currentUser.Email = model.Register.Email;
                currentUser.Birthday = model.Register.Birthday;

                if (!string.IsNullOrEmpty(model.Register.Password) && !string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (model.Register.Password == model.ConfirmPassword)
                    {
                        currentUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.";
                        return View("EditInfo", model);
                    }
                }

                try
                {
                    _db.Users.Update(currentUser);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật thông tin. Vui lòng thử lại.";
                    return View("EditInfo", model);
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            var menus = await _db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new UserViewModel
            {
                Menus = menus,
                Register = currentUser
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePassword(UserViewModel model)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Register.Password) && !string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (model.Register.Password == model.ConfirmPassword)
                    {
                        currentUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Register.Password);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.";
                        return View("EditInfo", model);
                    }
                }

                try
                {
                    _db.Users.Update(currentUser);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi cập nhật thông tin. Vui lòng thử lại.";
                    return View("ChangePassword", model);
                }
            }
        }

        private async Task<List<Menu>> GetMenusAsync()
        {
            return await _db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
        }

        private async Task SignInUser(User user)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
        }
    }
}