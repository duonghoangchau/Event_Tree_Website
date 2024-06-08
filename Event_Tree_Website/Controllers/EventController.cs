using System.Globalization;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class EventController : Controller
    {
        private readonly Event_TreeContext _context;
        public EventController(Event_TreeContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _context.Events.Where(m => m.Hide == 0).CountAsync(); // Tổng số sản phẩm

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // Tính tổng số trang

            var eves = await _context.Events.Where(m => m.Hide == 0)
                                                .OrderByDescending(m => m.DateTime)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync(); // Lấy sự kiện cho trang hiện tại

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

            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = eves,
                TotalPages = totalPages,
                CurrentPage = page
            };
            return View(viewModel);
        }
        public async Task<IActionResult> CateEvent(string slug, long id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var cateEvent = await _context.Categories.Where(cp => cp.IdCategory == id && cp.Link == slug).FirstOrDefaultAsync();
            if (cateEvent == null)
            {
                var errorViewModel = new ViewModels.ErrorViewModel
                {
                    RequestId = "CateProd Error",
                };
                return View("Error", errorViewModel);
            }
            var events = await _context.Events.Where(m => m.Hide == 0 && m.IdCategory == cateEvent.IdCategory)
            .OrderByDescending(m => m.DateTime).ToListAsync();
            var imageCodes = events.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in events)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }
            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                cateName = cateEvent.Name,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EventDetail(string slug, long id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var events = await _context.Events.Where(m => m.Link == slug && m.Id == id).ToListAsync();
            if (events == null)
            {
                var errorViewModel = new ViewModels.ErrorViewModel
                {
                    RequestId = "Event Error",
                };
                return View("Error", errorViewModel);
            }
            var imageCodes = events.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in events)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }
            var viewModel = new EventViewModel
            {
                Menus = menus,
                Images = images,
                Eves = events,
            };
            return View(viewModel);
        }
        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }


        public async Task<IActionResult> TimKiem(string keyword, int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _context.Events
                                            .Where(m => m.Hide == 0 && EF.Functions.Like(m.Name, $"%{keyword}%"))
                                            .CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var events = await _context.Events
                                       .Where(p => p.Hide == 0 && EF.Functions.Like(p.Name, $"%{keyword}%"))
                                       .Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();
            var imageCodes = events.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in events)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }
            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                cateName = "Kết quả tìm kiếm cho: " + keyword,
                CurrentPage = page,
                TotalPages = totalPages
            };

            ViewData["keyword"] = keyword;

            return View(viewModel);
        }



        ///////////////////////////////////////////////


        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string keyword)
        {
            // Chuyển từ khóa nhập vào thành dạng không dấu
            string keywordWithoutDiacritics = RemoveDiacritics(keyword).ToLower();

            // Tạo một danh sách các từ có dấu và không dấu
            var wordDictionary = new Dictionary<string, string>();
            var events = await _context.Events.Where(e => e.Hide == 0).ToListAsync();
            foreach (var item in events)
            {
                string eventNameWithoutDiacritics = RemoveDiacritics(item.Name).ToLower();
                if (!wordDictionary.ContainsKey(eventNameWithoutDiacritics))
                {
                    wordDictionary.Add(eventNameWithoutDiacritics, item.Name);
                }
            }

            // Tìm kiếm các từ gợi ý trong danh sách từ điển
            var suggestions = wordDictionary
                                .Where(entry => entry.Key.Contains(keywordWithoutDiacritics) || entry.Value.Contains(keyword))
                                .Select(entry => entry.Value)
                                .Take(10) // Chỉ lấy 10 gợi ý
                                .ToList();

            return Json(suggestions);
        }

        // Hàm chuẩn hóa văn bản bằng cách loại bỏ dấu
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public IActionResult Error()
        {
            var errorViewModel = new ViewModels.ErrorViewModel
            {
                // Khởi tạo các thuộc tính cần thiết
            };

            return View(errorViewModel);
        }
    }
}
