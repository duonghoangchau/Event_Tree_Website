
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Event_Tree_Website.Controllers
{
    public class PersonalEventManagementController : Controller
    {
        private readonly Event_TreeContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public PersonalEventManagementController(Event_TreeContext context, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            // Đếm tổng số sự kiện có Status = 1
            var totalItems = await _context.PersonalEvents.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy danh sách sự kiện có Status = 1 cho trang hiện tại
            var pers = await _context.PersonalEvents
                                     .OrderByDescending(m => m.DateTime)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            // Lấy danh sách menu không bị ẩn
            var menus = await _context.Menus
                                      .Where(m => m.Hide == 0)
                                      .OrderBy(m => m.MenuOrder)
                                      .ToListAsync();
            var imageCodes = pers.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in pers)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }
            // Tạo ViewModel
            var viewModel = new PersonalEventManagementViewModel
            {
                Menus = menus,
                Pers = pers,
                TotalPages = totalPages,
                CurrentPage = page,
            };

            // Trả về view với PersonalEventViewModel

            return View(viewModel);
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
        public async Task<IActionResult> Search(int id)
        {
            var per = await _context.PersonalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (per == null)
            {
                return RedirectToAction("Index");
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new PersonalEventManagementViewModel
            {
                Menus = menus,
                Images = _context.Images.Where(i => i.ImageCode.Equals(per.ImageCode)).ToList(),
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
            var imageCodes = pers.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in pers)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new PersonalEventManagementViewModel
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
            var viewModel = new PersonalEventManagementViewModel
            {
                Personals = personals,
                Images = _context.Images.Where(i => i.ImageCode.Equals(personals.ImageCode)).ToList(),
                Menus = menus
            };
            return View(viewModel);
        }


        private bool SukienExists(int id)
        {
            return _context.PersonalEvents.Any(e => e.Id == id);
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
            var hideOptions = new List<SelectListItem>();
            hideOptions.Add(new SelectListItem { Value = "0", Text = "Hiển Thị" });
            hideOptions.Add(new SelectListItem { Value = "1", Text = "Ẩn" });
            // Tạo view model và truyền dữ liệu
            var viewModel = new PersonalEventManagementViewModel
            {
                Menus = menus,
                Personals = sukien,
                HideOptions = hideOptions
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonalEvent personals)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var personal1 = await _context.PersonalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (personal1 == null)
            {
                return NotFound();
            }
            var hideOptions = new List<SelectListItem>();
            hideOptions.Add(new SelectListItem { Value = "0", Text = "Hiển Thị" });
            hideOptions.Add(new SelectListItem { Value = "1", Text = "Ẩn" });
            var viewModel = new PersonalEventManagementViewModel
            {
                Menus = menus,
                Personals = personal1,
                HideOptions = hideOptions

            };
            if (id != viewModel.Personals.Id)
            {

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    var existingPersonalEvent = await _context.PersonalEvents.FindAsync(id);
                    if (existingPersonalEvent == null)
                    {
                        return NotFound();
                    }


                    // Cập nhật các trường của existingCatology với giá trị từ catology được gửi từ form

                    existingPersonalEvent.Hide = personals.Hide;
                    existingPersonalEvent.UpdatedAt = DateTime.Now;




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
        public async Task<IActionResult> Gmail()
        {

            // Lấy danh sách menu không bị ẩn
            var menus = await _context.Menus
                                      .Where(m => m.Hide == 0)
                                      .OrderBy(m => m.MenuOrder)
                                      .ToListAsync();
            var viewModel = new PersonalEventManagementViewModel
            {
                Menus = menus
            };

            // Trả về view với PersonalEventViewModel

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SendEmailNotification()
        {
            try
            {
                // Lấy danh sách các sự kiện cá nhân sắp diễn ra trong 7 ngày tới
                var upcomingEvents = await _context.PersonalEvents
                    .Where(e => e.DateTime >= DateTime.Today && e.DateTime <= DateTime.Today.AddDays(7))
                    .ToListAsync();

                // Lặp qua từng sự kiện và gửi email thông báo
                foreach (var personalEvent in upcomingEvents)
                {
                    // Lấy thông tin người dùng từ Id_user trong personalEvent
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == personalEvent.IdUser);

                    // Gửi email thông báo cho người dùng
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        string subject = "Thông báo sự kiện sắp diễn ra";
                        string body = $"Sự kiện '{personalEvent.Name}' sẽ diễn ra vào ngày {personalEvent.DateTime.ToShortDateString()}.";

                        // Gửi email
                        SendEmail(user.Email, subject, body);
                    }
                }

                return Json(new { success = true, message = "Emails đã được gửi thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi gửi email: " + ex.Message });
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            // Đọc cấu hình email từ tệp appsettings.json
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var emailPassword = _configuration["EmailSettings:EmailPassword"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            // Tạo đối tượng MailMessage
            MailMessage mail = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // Tạo đối tượng SmtpClient để gửi email
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromEmail, emailPassword),
                EnableSsl = true // Sử dụng SSL để bảo vệ email
            };

            // Gửi email
            smtpClient.Send(mail);
        }
    }
}
