using System.Globalization;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using Imgur.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Image = Event_Tree_Website.Models.Image;

namespace Event_Tree_Website.Controllers
{
    [Authorize(Roles = "1,2")]
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
            var totalItems = await _context.Events.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var eves = await _context.Events.OrderByDescending(m => m.DateTime)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var imageCodes = eves.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in eves)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }

            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Eves = eves,
                TotalPages = totalPages,
                CurrentPage = page,

            };

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
        public async Task<IActionResult> Create()
        {
            showDropList();
            var menus = await _context.Menus.Where(m => m.Hide == 0).ToListAsync();
            var viewModel = new EventManagementViewModel { Menus = menus };
            return View(viewModel);
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
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var apiClient = new ApiClient("6efaec52e38d148", "5de13ec766f236d3d39808cb21fec395962922cf");
                        var httpClient = new HttpClient();

                        var oAuth2Endpoint = new OAuth2Endpoint(apiClient, httpClient);
                        var authUrl = oAuth2Endpoint.GetAuthorizationUrl();

                        var token = new OAuth2Token
                        {
                            AccessToken = "4e5b5d07a81334ea5c5459dc5c1ef63458c296eb",
                            RefreshToken = "6e653d2f3b7c99cb1c135f690c5b297b8a1555b1",
                            AccountId = 180393165,
                            AccountUsername = "lvxadoniss1",
                            ExpiresIn = 315360000,
                            TokenType = "bearer"
                        };

                        apiClient.SetOAuth2Token(token);
                        var imageEndpoint = new ImageEndpoint(apiClient, httpClient);
                        var code = "IMG_" + GenerateRandomString(8);

                        foreach (var file in files)
                        {
                            if (file != null && file.Length > 0)
                            {
                                using var fileStream = file.OpenReadStream();
                                var imageUpload = await imageEndpoint.UploadImageAsync(fileStream);
                                var imageUrl = imageUpload.Link;

                                var newImage = new Image
                                {
                                    Url = imageUrl,
                                    Type = 2,
                                    ImageCode = code,
                                };
                                events.ImageCode = code;
                                _context.Images.Add(newImage);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                _context.Events.Add(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var menus = await _context.Menus.Where(m => m.Hide == 0).ToListAsync();
            var viewModel = new EventManagementViewModel { Menus = menus, Events = events };
            return View(viewModel);
        }

        public static string GenerateRandomString(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                builder.Append(chars[index]);
            }
            return builder.ToString();
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
                Images = _context.Images.Where(i => i.ImageCode.Equals(eve.ImageCode)).ToList(),
                Events = eve
            };
            return RedirectToAction("Edit", new { id = eve.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }
            string keywordWithoutDiacritics = RemoveDiacritics(keyword);
            var eves = await _context.Events
                .Where(p => p.Name.Contains(keyword) || p.Name.Contains(keywordWithoutDiacritics))
                .OrderBy(m => m.DateTime)
                .ToListAsync();
            var imageCodes = eves.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in eves)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Eves = eves,
                cateName = keyword
            };

            return View("Index", viewModel);
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

            var images = _context.Images.Where(i => i.ImageCode.Equals(events.ImageCode)).ToList();

            var viewModel = new EventManagementViewModel
            {
                Events = events,
                Images = images,
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
            var viewModel = new EventManagementViewModel
            {
                Menus = menus,
                Events = sukien,
                Images = _context.Images.Where(i => i.ImageCode.Equals(sukien.ImageCode)).ToList(),
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
                                var apiClient = new ApiClient("6efaec52e38d148", "5de13ec766f236d3d39808cb21fec395962922cf");
                                var httpClient = new HttpClient();

                                var oAuth2Endpoint = new OAuth2Endpoint(apiClient, httpClient);
                                var authUrl = oAuth2Endpoint.GetAuthorizationUrl();

                                var token = new OAuth2Token
                                {
                                    AccessToken = "4e5b5d07a81334ea5c5459dc5c1ef63458c296eb",
                                    RefreshToken = "6e653d2f3b7c99cb1c135f690c5b297b8a1555b1",
                                    AccountId = 180393165,
                                    AccountUsername = "lvxadoniss1",
                                    ExpiresIn = 315360000,
                                    TokenType = "bearer"
                                };

                                apiClient.SetOAuth2Token(token);
                                var imageEndpoint = new ImageEndpoint(apiClient, httpClient);
                                var code = "IMG_" + GenerateRandomString(8);

                                foreach (var file in files)
                                {
                                    if (file != null && file.Length > 0)
                                    {
                                        using var fileStream = file.OpenReadStream();
                                        var imageUpload = await imageEndpoint.UploadImageAsync(fileStream);
                                        var imageUrl = imageUpload.Link;

                                        var newImage = new Image
                                        {
                                            Url = imageUrl,
                                            Type = 2,
                                            ImageCode = code,
                                        };
                                        events.ImageCode = code;
                                        _context.Images.Add(newImage);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                    var existingEvent = await _context.Events.FindAsync(id);
                    if (existingEvent == null)
                    {
                        return NotFound();
                    }
                    existingEvent.Name = events.Name;
                    existingEvent.DateTime = events.DateTime;
                    existingEvent.Description = events.Description;
                    existingEvent.Detail = events.Detail;
                    existingEvent.ImageCode = events.ImageCode;
                    existingEvent.Link = events.Link;
                    existingEvent.Hide = events.Hide;
                    existingEvent.UpdatedAt = DateTime.Now;
                    existingEvent.DeletedAt = DateTime.Now;
                    if (!string.IsNullOrEmpty(events.ImageCode))
                    {
                        existingEvent.ImageCode = events.ImageCode;
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sự kiện thành công.";
                    return RedirectToAction("Index");
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
            return View(viewModel);
        }
    }
}
