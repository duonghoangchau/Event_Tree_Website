using System.Globalization;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class EventManagementController : Controller
    {
        private readonly Event_TreeContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public EventManagementController(Event_TreeContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        private void showDropList()
        {
            List<SelectListItem> list = _context.Categories
                                            .Select(c => new SelectListItem
                                            {
                                                Text = c.Name,
                                                Value = c.IdCategory.ToString()
                                            })
                                            .Distinct()
                                            .ToList();
            ViewBag.IdCategory = list;
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _context.Events.CountAsync(); // Tổng số sản phẩm

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // Tính tổng số trang

            var eves = await _context.Events.OrderByDescending(m => m.DateTime)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync(); // Lấy sự kiện cho trang hiện tại

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();



            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Eves = eves,
                TotalPages = totalPages,
                CurrentPage = page,

            };

            return View(viewModel); // Trả về view với AdminViewModel
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        private void showHideDropdownList()
        {
            var hideOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Hiển thị", Value = "0" },
                new SelectListItem { Text = "Ẩn", Value = "1" }
            };

            ViewBag.HideOptions = hideOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            showDropList();
            var menus = await _context.Menus.Where(m => m.Hide == 0).ToListAsync();
            return View(new EventManagementViewModel { Menus = menus });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event events, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                events.Hide = 0;
                events.CreatedAt = DateTime.Now;
                events.UpdatedAt = DateTime.Now;
                events.DeletedAt = DateTime.Now;
                showHideDropdownList();
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
                        events.ImageCode = filePaths[0];
                }

                // Tạo một đối tượng AdminViewModel từ Product
                var viewModel = new EventManagementViewModel
                {
                    Events = events // gán product vào AdminViewModel
                };
                _context.Add(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //showDropList();
            return View(events);
        }


        [HttpGet]
        public async Task<IActionResult> Search(int id)
        {
            var eve = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (eve == null)
            {

                return RedirectToAction("Index");
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Events = eve
            };

            // Chuyển hướng đến trang Edit với IdCart tương ứng
            return RedirectToAction("Edit", new { id = eve.Id });
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
            var eves = await _context.Events
                .Where(p => p.Name.Contains(keyword) || p.Name.Contains(keywordWithoutDiacritics))
                .OrderBy(m => m.DateTime)
                .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Eves = eves,
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

            var events = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (events == null)
            {
                return NotFound();
            }
            var viewModel = new EventManagementViewModel
            {
                Events =events,
                Menus = menus
            };
            return View(viewModel);
        }


        private bool SukienExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sukien = await _context.Events.FindAsync(id);
            if (sukien == null)
            {
                return NotFound();
            }
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var hideOptions = new List<SelectListItem>();
            hideOptions.Add(new SelectListItem { Value = "0", Text = "Hiển Thị" });
            hideOptions.Add(new SelectListItem { Value = "1", Text = "Ẩn" });
            // Tạo view model và truyền dữ liệu
            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Events = sukien,
                HideOptions = hideOptions
            };
            showDropList();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateTime,Description,Detail,ImageCode,Link,Hide,Status")] Event events, List<IFormFile> files)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var event1 = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (event1 == null)
            {
                return NotFound();
            }
            var hideOptions = new List<SelectListItem>();
            hideOptions.Add(new SelectListItem { Value = "0", Text = "Hiển Thị" });
            hideOptions.Add(new SelectListItem { Value = "1", Text = "Ẩn" });
            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Events = event1,
                HideOptions = hideOptions

            };
            if (id != viewModel.Events.Id)
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
                            events.ImageCode = filePaths[0];
                        }

                    }

                    var existingEvent = await _context.Events.FindAsync(id);

                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường của existingCatology với giá trị từ catology được gửi từ form
                    existingEvent.Name = events.Name;
                    existingEvent.DateTime = events.DateTime;
                    existingEvent.Description = events.Description;
                    existingEvent.Detail = events.Detail;
                    existingEvent.ImageCode = events.ImageCode;
                    existingEvent.Link = events.Link;
                    existingEvent.Hide = events.Hide;
                    existingEvent.Status = events.Status;
                    existingEvent.UpdatedAt = DateTime.Now;
                    existingEvent.DeletedAt = DateTime.Now;
                    if (!string.IsNullOrEmpty(events.ImageCode))
                    {
                        existingEvent.ImageCode = events.ImageCode;
                    }


                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sự kiện thành công.";
                    return RedirectToAction("Index"); // Điều hướng đến trang chính sau khi chỉnh sửa thành công
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SukienExists(viewModel.Events.Id))
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
