using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly Event_TreeContext db;
        public AccountManagementController(Event_TreeContext _db)
        {
            db = _db;
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await db.Users.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await db.Users.Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();
            var menus = await db.Menus.Where(m => m.Hide == 0).ToListAsync();

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

            // Lấy thông tin đơn hàng từ database
            var user = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var menus = await db.Menus.Where(m => m.Hide == 0).ToListAsync();
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
            var menus = await db.Menus.Where(m => m.Hide == 0).ToListAsync();
            var user1 = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user1 == null)
            {
                return NotFound();
            }
            var viewModel = new AccountViewModel
            {
                Menus = menus,
                User = user1
            };
            if (id != viewModel.User.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //    // Cập nhật thông tin đơn hàng vào cơ sở dữ liệu
                    //    _context.Update(viewModel.Catology); // Cập nhật thông tin đơn hàng
                    //    await _context.SaveChangesAsync(); // Lưu các thay đổi vào cơ sở dữ liệu
                    var existingCatology = await db.Users.FindAsync(id);

                    if (existingCatology == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường của existingCatology với giá trị từ catology được gửi từ form
                    existingCatology.Role = user.Role;
                    existingCatology.Status = user.Status;


                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                    return RedirectToAction("Index"); // Điều hướng đến trang chính sau khi chỉnh sửa thành công
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

            // Tạo view model và truyền dữ liệu

            return View(viewModel);
        }
        private bool UserExists(int id)
        {
            return db.Users.Any(e => e.Id == id);
        }
    }
}
