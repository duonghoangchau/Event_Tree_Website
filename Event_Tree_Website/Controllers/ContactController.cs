using Event_Tree_Website.Models;
using Event_Tree_Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
	public class ContactController : Controller
	{
		private readonly Event_TreeContext _context;
		public ContactController(Event_TreeContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{

			var menus = await _context.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();

			var viewModel = new ContactViewModel
			{
				Menus = menus

			};
			return View(viewModel);
		}
		public async Task<IActionResult> _MenuPartial()
		{
			return PartialView();
		}
		public async Task<IActionResult> _BlogPartial()
		{
			return PartialView();
		}
	}
}
