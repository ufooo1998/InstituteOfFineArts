using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Identity;
using InstituteOfFineArts.Areas.Identity.Data;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace InstituteOfFineArts.Controllers
{
    public class HomeController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;
        private readonly UserManager<CustomUser> _userManager;

        public HomeController(UserManager<CustomUser> userManager, InstituteOfFineArtsContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Competition()
        {
            return View();
        }

        public IActionResult Attend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attend([Bind("ID,PostName,Decription,Price")] Post post, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var currentCompetition = _context.Competition.Where(c => c.Status == CompetitonStatus.During).Single();

                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    post.Image = ms.ToArray();
                }

                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.UserID = user.Id;
                _context.Add(post);
                _context.CompetitionPost.Add(new CompetitionPost { CompetitionID = currentCompetition.ID, PostID = post.ID, SubmitDate = DateTime.Now });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public IActionResult Guide()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
