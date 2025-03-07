﻿using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly Event_TreeContext _context;

        public HomeController(Event_TreeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var slides = await _context.Sliders.Where(m => m.Hide == 0).OrderBy(m => m.Order).ToListAsync();

            var categoriesParent = await _context.Categories.Where(m => (m.Hide == 0 || m.Hide == null) && (m.IdParent == "0" || m.IdParent == null)).ToListAsync();


            List<Category> categories = _context.Categories.Where(x => x.Hide == 0 || x.Hide == null).ToList();

            List<Event> even = _context.Events.ToList();
            var imageCodes = even.Select(c => c.ImageCode).Distinct().ToList();

            var images = _context.Images.Where(i => imageCodes.Contains(i.ImageCode)).ToList();

            foreach (var eve in even)
            {
                var image = images.FirstOrDefault(i => i.ImageCode == eve.ImageCode);
                if (image != null)
                {
                    eve.ImageCode = image.Url;
                }
            }
            var viewModel = new HomeViewModel
            {
                Menus = menus,
                Sliders = slides,
                CategoriesParent = categoriesParent,
                Categories = categories,
                Even = even,

            };
            return View(viewModel);
        }


        public async Task<IActionResult> _MenuPartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _SlidePartial()
        {
            return PartialView();
        }
        public async Task<IActionResult> _EventPartial()
        {
            return PartialView();
        }
    }
}