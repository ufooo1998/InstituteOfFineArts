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
using Microsoft.EntityFrameworkCore;

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
            ViewData["During"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.During).OrderBy(c => c.CreatedAt).ToList();
            ViewData["ComingUp"] = _context.Competition.Where(c => c.Status == CompetitonStatus.ComingUp).OrderBy(c => c.CreatedAt).ToList();
            return View();
        }

        public IActionResult Competition()
        {
            ViewData["During"] = _context.Competition.Include(a=>a.CompetitionPosts).Where(c=>c.Status == CompetitonStatus.During).OrderBy(c=>c.CreatedAt).ToList();
            ViewData["ComingUp"] = _context.Competition.Where(c => c.Status == CompetitonStatus.ComingUp).OrderBy(c=>c.CreatedAt).ToList();
            ViewData["Examining"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Examining).OrderBy(c=>c.CreatedAt).ToList();
            ViewData["Ended"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Ended).OrderBy(c=>c.CreatedAt).ToList();
            return View();
        }

        public async Task<IActionResult> CompetitionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var competition = await _context.Competition.Include(a=>a.User).FirstOrDefaultAsync(m => m.ID == id);
            var posts = _context.CompetitionPost.Include(a => a.Post).Where(c=>c.CompetitionID == id).ToList();
            ViewData["Posts"] = posts;
            return View(competition);
        }
        public async Task<IActionResult> Attend()
        {
            var user = await GetCurrentUserAsync();
            ViewData["UserSignedIn"] = false;
            if (user != null)
            {
                var IsStudentRole = _userManager.IsInRoleAsync(user, "Student");
                ViewData["UserSignedIn"] = IsStudentRole.Result;
            }
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
                var userPostsCount = _context.CompetitionPost.Where(c => c.UserID == user.Id && c.CompetitionID == currentCompetition.ID).Count();
                if (userPostsCount == 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Image.CopyTo(ms);
                        post.Image = ms.ToArray();
                    }

                    post.CreatedAt = DateTime.Now;
                    post.UpdatedAt = DateTime.Now;
                    post.UserID = user.Id;
                    _context.Add(post);
                    _context.CompetitionPost.Add(new CompetitionPost { CompetitionID = currentCompetition.ID, PostID = post.ID, UserID = user.Id, SubmitDate = DateTime.Now });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyAccount));
                }
                TempData["PostError"] = "You already have post on this competition!";
                return new JsonResult(TempData["PostError"]);
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

        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            var userPosts = _context.Post.Where(c=>c.UserID == user.Id).ToList();
            return View(userPosts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
