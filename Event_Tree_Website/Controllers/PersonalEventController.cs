
using System.Globalization;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class PersonalEventController : Controller
    {
        private readonly Event_TreeContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public PersonalEventController(Event_TreeContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            // Lấy id của người dùng hiện tại
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                // Nếu không có người dùng đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Đếm tổng số sự kiện có Status = 1 và id_user = id của người dùng hiện tại
            var totalItems = await _context.PersonalEvents
                                            .Where(pe => pe.IdUser.ToString() == userId)
                                            .CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy danh sách sự kiện có Status = 1 và id_user = id của người dùng hiện tại cho trang hiện tại
            var pers = await _context.PersonalEvents
                                    .Where(pe => pe.IdUser.ToString() == userId && pe.Hide == 0)
                                     .OrderByDescending(m => m.DateTime)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            // Lấy danh sách menu không bị ẩn
            var menus = await _context.Menus
                                      .Where(m => m.Hide == 0)
                                      .OrderBy(m => m.MenuOrder)
                                      .ToListAsync();

            // Tạo ViewModel
            var viewModel = new PersonalEventViewModel
            {
                Menus = menus,
                Pers = pers,
                TotalPages = totalPages,
                CurrentPage = page,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).ToListAsync();
            return View(new PersonalEventViewModel { Menus = menus });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonalEvent personals, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                // Lấy id của người dùng hiện tại từ session
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    // Nếu không có người dùng đăng nhập, chuyển hướng đến trang đăng nhập
                    return RedirectToAction("Login", "Account");
                }

                // Gán id_user của người đăng nhập cho sự kiện mới
                personals.IdUser = int.Parse(userId);

                // Tiếp tục tạo sự kiện như bình thường
                personals.Hide = 0;
                personals.CreatedAt = DateTime.Now;
                personals.UpdatedAt = DateTime.Now;

                //showHideDropdownList();
                if (files != null && files.Count > 0)
                {
                    var filePaths = new List<string>();
                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(formFile.FileName);
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            filePaths.Add(fileName);
                        }
                    }
                    // Gán đường dẫn của ảnh tải lên cho các trường tương ứng trong đối tượng Product
                    if (filePaths.Count >= 1)
                        personals.ImageCode = filePaths[0];
                }

                // Tạo một đối tượng AdminViewModel từ Product
                var viewModel = new PersonalEventViewModel
                {
                    Personals = personals // gán product vào AdminViewModel
                };
                _context.Add(personals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //showDropList();
            return View(personals);
        

           
        }


        [HttpGet]
        public async Task<IActionResult> Search(int id)
        {
            var per = await _context.PersonalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (per == null)
            {
                return RedirectToAction("Index");
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new PersonalEventViewModel
            {
                Menus = menus,
                Personals = per
            };

            // Chuyển hướng đến trang Edit với IdCart tương ứng
            return RedirectToAction("Edit", new { id = per.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu từ khóa trống, hiển thị tất cả sản phẩm
                return RedirectToAction("Index");
            }

            // Tạo phiên bản không dấu của từ khóa tìm kiếm
            string keywordWithoutDiacritics = RemoveDiacritics(keyword);

            // Tìm kiếm cả từ có dấu và không dấu
            var pers = await _context.PersonalEvents
                .Where(p => p.Name.Contains(keyword) || p.Name.Contains(keywordWithoutDiacritics))
                .OrderBy(m => m.DateTime)
                .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new PersonalEventViewModel
            {
                Menus = menus,
                Pers = pers,
                cateName = keyword // Dùng từ khóa có dấu để hiển thị lại trên giao diện
            };

            return View("Index", viewModel); // Trả về view Index với dữ liệu đã lọc
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            text = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in text)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }




        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            if (id == null)
            {
                return NotFound();
            }

            var personals = await _context.PersonalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (personals == null)
            {
                return NotFound();
            }
            var viewModel = new PersonalEventViewModel
            {
                Personals = personals,
                Menus = menus
            };
            return View(viewModel);
        }


        private bool SukienExists(int id)
        {
            return _context.PersonalEvents.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var personalEvent = await _context.PersonalEvents.FindAsync(id);
            if (personalEvent == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái "Hide" thành 1
            personalEvent.Hide = 1;
            _context.PersonalEvents.Update(personalEvent);
            await _context.SaveChangesAsync();

            // Trả về một JSON cho phía client
            return Json(new { success = true });
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sukien = await _context.PersonalEvents.FindAsync(id);
            if (sukien == null)
            {
                return NotFound();
            }
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            // Tạo view model và truyền dữ liệu
            var viewModel = new PersonalEventViewModel
            {
                Menus = menus,
                Personals = sukien,
                //HideOptions = hideOptions
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateTime,Description,Detail,ImageCode,Link,Hide")] PersonalEvent personals, List<IFormFile> files)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var personal1 = await _context.PersonalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (personal1 == null)
            {
                return NotFound();
            }

            var viewModel = new PersonalEventViewModel
            {
                Menus = menus,
                Personals = personal1,
                //HideOptions = hideOptions

            };
            if (id != viewModel.Personals.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (files != null && files.Count > 0)
                    {
                        var filePaths = new List<string>();
                        foreach (var formFile in files)
                        {
                            if (formFile.Length > 0)
                            {
                                var fileName = Path.GetFileName(formFile.FileName);
                                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                                filePaths.Add(fileName);
                            }
                        }
                        // Gán đường dẫn của ảnh tải lên cho các trường tương ứng trong đối tượng Product
                        if (filePaths.Count >= 1)
                        {
                            // Không cần thêm tiền tố "images\" vào đường dẫn
                            personals.ImageCode = filePaths[0];
                        }

                    }

                    var existingPersonalEvent = await _context.PersonalEvents.FindAsync(id);

                    if (existingPersonalEvent == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường của existingCatology với giá trị từ catology được gửi từ form
                    existingPersonalEvent.Name = personals.Name;
                    existingPersonalEvent.DateTime = personals.DateTime;
                    existingPersonalEvent.Description = personals.Description;
                    existingPersonalEvent.Detail = personals.Detail;
                    existingPersonalEvent.ImageCode = personals.ImageCode;
                    existingPersonalEvent.Link = personals.Link;
                    existingPersonalEvent.Hide = 0;
                    existingPersonalEvent.UpdatedAt = DateTime.Now;

                    if (!string.IsNullOrEmpty(personals.ImageCode))
                    {
                        existingPersonalEvent.ImageCode = personals.ImageCode;
                    }


                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sự kiện thành công.";
                    return RedirectToAction("Index"); // Điều hướng đến trang chính sau khi chỉnh sửa thành công
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SukienExists(viewModel.Personals.Id))
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

   

    }
}
