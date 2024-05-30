using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Text;

namespace Event_Tree_Website.Controllers
{
    public class PremiumController : Controller
    {
        private readonly Event_TreeContext _context;
        public PremiumController(Event_TreeContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            var pres = await _context.Premiums
            .OrderBy(m => m.PremiumLevel).ToListAsync();
            var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var viewModel = new PremiumViewModel
            {
                Menus = menus,
                Pres = pres,
            };
            return View(viewModel);
        }
        public ActionResult Buy()
        {
            return RedirectToAction("Index", "Course");
        }
    }
}
