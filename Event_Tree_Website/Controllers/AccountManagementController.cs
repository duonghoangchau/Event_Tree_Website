using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    [Authorize(Roles = "2")]
    public class AccountManagementController : Controller
    {
        private readonly Event_TreeContext _db;

        public AccountManagementController(Event_TreeContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _db.Users.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await _db.Users.Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();
            var menus = await _db.Menus.Where(m => m.Hide == 0).ToListAsync();

            var viewModel = new AccountViewModel
            {
                Users = users,
                Menus = menus,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var menus = await _db.Menus.Where(m => m.Hide == 0).ToListAsync();
            var hideOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Hiển Thị" },
                new SelectListItem { Value = "false", Text = "Ẩn" }
            };
            var perOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Khách hàng" },
                new SelectListItem { Value = "1", Text = "Quản Lý" },
                new SelectListItem { Value = "2", Text = "Admin" }
            };

            var viewModel = new AccountViewModel
            {
                Menus = menus,
                User = user,
                HideOptions = hideOptions,
                PerOptions = perOptions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Fullname,Email,Birthday,Role,Status")] User user)
        {
            var menus = await _db.Menus.Where(m => m.Hide == 0).ToListAsync();
            var existingUser = await _db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var viewModel = new AccountViewModel
            {
                Menus = menus,
                User = existingUser
            };

            if (id != viewModel.User.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingUser.Role = user.Role;
                    existingUser.Status = user.Status;

                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(viewModel.User.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(viewModel);
        }

        private bool UserExists(int id)
        {
            return _db.Users.Any(e => e.Id == id);
        }
    }
}
