using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

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
            var totalItems = await _context.Events.Where(m => m.Hide == 0).CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var events = await _context.Events.Where(m => m.Hide == 0)
                                                .OrderByDescending(m => m.DateTime)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var images = await LoadImagesForEvents(events);

            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                Images = images,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CateEvent(string slug, long id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var cateEvent = await _context.Categories.FirstOrDefaultAsync(cp => cp.IdCategory == id && cp.Link == slug);
            if (cateEvent == null)
            {
                return RedirectToAction("Error");
            }

            var events = await _context.Events.Where(m => m.Hide == 0 && m.IdCategory == cateEvent.IdCategory)
                                                .OrderByDescending(m => m.DateTime)
                                                .ToListAsync();

            var images = await LoadImagesForEvents(events);

            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                cateName = cateEvent.Name,
                Images = images
            };

            return View(viewModel);
        }

        public async Task<IActionResult> EventDetail(string slug, long id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var events = await _context.Events.Where(m => m.Link == slug && m.Id == id).ToListAsync();
            if (events == null)
            {
                return RedirectToAction("Error");
            }

            var images = await LoadImagesForEvents(events);

            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                Images = images
            };

            return View(viewModel);
        }

        public async Task<IActionResult> TimKiem(string keyword, int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _context.Events.Where(m => m.Hide == 0 && EF.Functions.Like(m.Name, $"%{keyword}%")).CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var events = await _context.Events.Where(p => p.Hide == 0 && EF.Functions.Like(p.Name, $"%{keyword}%"))
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            var images = await LoadImagesForEvents(events);

            var viewModel = new EventViewModel
            {
                Menus = menus,
                Eves = events,
                cateName = "Kết quả tìm kiếm cho: " + keyword,
                CurrentPage = page,
                TotalPages = totalPages,
                Images = images
            };

            ViewData["keyword"] = keyword;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string keyword)
        {
            string keywordWithoutDiacritics = RemoveDiacritics(keyword).ToLower();

            var wordDictionary = await CreateWordDictionary();

            var suggestions = wordDictionary
                                .Where(entry => entry.Key.Contains(keywordWithoutDiacritics) || entry.Value.Contains(keyword))
                                .Select(entry => entry.Value)
                                .Take(10)
                                .ToList();

            return Json(suggestions);
        }

        private async Task<List<Image>> LoadImagesForEvents(List<Event> events)
        {
            var imageCodes = events.Select(c => c.ImageCode).Distinct().ToList();
            var images = await _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToListAsync();

            foreach (var eve in events)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }

            return images;
        }

        private async Task<Dictionary<string, string>> CreateWordDictionary()
        {
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

            return wordDictionary;
        }

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
            var errorViewModel = new ViewModels.ErrorViewModel{};
            return View(errorViewModel);
        }
    }
}
