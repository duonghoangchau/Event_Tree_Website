using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using Imgur.API.Models;
using System.Net.Http;
using System.Text;
using Image = Event_Tree_Website.Models.Image;
using Event_Tree_Website.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Event_Tree_Website.Controllers
{
    public class ImageController : Controller
    {
        Event_TreeContext db = new Event_TreeContext();

        public IActionResult Index()
        {
            var menus = db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToList();
            var images = db.Images.ToList();

            var viewModel = new ImageViewModel
            {
                Menus = menus,
                Images = images
            };

            return View(viewModel);
        }

        public ActionResult Insert()
        {
            var menus = db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToList();
            var images = db.Images.ToList();

            var viewModel = new ImageViewModel
            {
                Menus = menus,
                Images = images
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(IEnumerable<IFormFile> files, string CategorySelection, bool? Category)
        {
            var menus = await db.Menus.Where(m => m.Hide == 0).OrderBy(m => m.MenuOrder).ToListAsync();
            var images = await db.Images.ToListAsync();
            var viewModel = new ImageViewModel
            {
                Menus = menus,
                Images = images

            };
            try
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
                        int? categoryValue = null;
                        if (int.TryParse(CategorySelection, out int result))
                        {
                            categoryValue = result;
                        }

                        var newImage = new Image
                        {
                            Url = imageUrl,
                            Type = categoryValue,
                            ImageCode = code,
                        };
                        db.Images.Add(newImage);
                        await db.SaveChangesAsync();
                    }
                }

                ViewBag.Message = "Image uploaded successfully!";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View(viewModel);
            }
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
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var image = await db.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (image != null)
            {
                db.Images.Remove(image);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Image deleted successfully!" });
            }
            return Json(new { success = false, message = "Image not found!" });
        }

    }
}
