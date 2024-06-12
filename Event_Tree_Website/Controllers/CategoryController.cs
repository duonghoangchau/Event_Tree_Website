using System.Globalization;
using System.Text;
using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    [Authorize(Roles = "1,2")]
    public class CategoryController : Controller
    {
        private readonly Event_TreeContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CategoryController(Event_TreeContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var totalItems = await _context.Categories.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var cats = await _context.Categories
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new CategoryViewModel
            {
                Cats = cats,
                Menus = menus,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            return View(new CategoryViewModel { Menus = menus });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            category.Hide = 0;
            category.IdParent = "0";

            _context.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Category = category,
                Menus = menus
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

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var hideOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Hiển Thị" },
                new SelectListItem { Value = "1", Text = "Ẩn" }
            };

            var viewModel = new CategoryViewModel
            {
                Menus = menus,
                Category = category,
                HideOptions = hideOptions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategory,Name,Order,Link,Hide,IdParent")] Category category)
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Menus = menus,
                Category = category
            };

            if (id != viewModel.Category.IdCategory)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Order = category.Order;
                    existingCategory.Link = category.Link;
                    existingCategory.Hide = category.Hide;
                    existingCategory.IdParent = category.IdParent;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucExists(viewModel.Category.IdCategory))
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

        private bool DanhMucExists(int id)
        {
            return _context.Categories.Any(e => e.IdCategory == id);
        }

        [HttpGet]
        public async Task<IActionResult> Search(int id)
        {
            var cat = await _context.Categories.FirstOrDefaultAsync(m => m.IdCategory == id);
            if (cat == null)
            {
                return RedirectToAction("Index");
            }

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new CategoryViewModel
            {
                Menus = menus,
                Categorys = cat
            };

            return RedirectToAction("Edit", new { id = cat.IdCategory });
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            string keywordWithoutDiacritics = RemoveDiacritics(keyword);

            var cats = await _context.Categories
                .Where(p => p.Name.Contains(keyword) || p.Name.Contains(keywordWithoutDiacritics))
                .OrderBy(m => m.Order)
                .ToListAsync();

            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

            var viewModel = new CategoryViewModel
            {
                Menus = menus,
                Cats = cats,
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
    }
}
