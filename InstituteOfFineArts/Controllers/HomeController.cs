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
using Microsoft.AspNetCore.Authorization;

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
            ViewData["During"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.During).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["ComingUp"] = _context.Competition.Where(c => c.Status == CompetitonStatus.ComingUp).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["Examining"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Examining).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["Ended"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Ended).OrderByDescending(c => c.CreatedAt).ToList();

            return View();
        }

        public IActionResult Competition()
        {
            ViewData["During"] = _context.Competition.Include(a=>a.CompetitionPosts).Where(c=>c.Status == CompetitonStatus.During).OrderByDescending(c=>c.CreatedAt).ToList();
            ViewData["ComingUp"] = _context.Competition.Where(c => c.Status == CompetitonStatus.ComingUp).OrderByDescending(c=>c.CreatedAt).ToList();
            ViewData["Examining"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Examining).OrderByDescending(c=>c.CreatedAt).ToList();
            ViewData["Ended"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Ended).OrderByDescending(c=>c.CreatedAt).ToList();

            var competitionList = _context.Competition.OrderByDescending(a=>a.CreatedAt).ToList();
            return View(competitionList);
        }

        public async Task<IActionResult> CompetitionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // check is current user in student role
            var user = await GetCurrentUserAsync();
            ViewData["IsStudent"] = false;
            if (user != null)
            {
                var IsStudentRole = _userManager.IsInRoleAsync(user, "Student");
                ViewData["IsStudent"] = IsStudentRole.Result;
            }

            // check number post of user
            if (user != null)
            {
                var userPost = _context.CompetitionPost.Where(a => a.UserID == user.Id && a.CompetitionID == id).Count();
                ViewData["IsValid"] = false;
                if (userPost == 0)
                {
                    ViewData["IsValid"] = true;
                }
            }
            

            // get list of post and its information
            var competition = await _context.Competition.Include(a=>a.User).FirstOrDefaultAsync(m => m.ID == id);
            var posts = _context.CompetitionPost.Include(a => a.Post).ThenInclude(b=>b.User).Where(c=>c.CompetitionID == id).ToList();
            ViewData["Posts"] = posts;
            return View(competition);
        }
        [Authorize(Roles = "Student")]
        public IActionResult Attend(int id)
        {
            ViewData["id"] = id;
            return View();
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attend([Bind("PostName,Decription")] Post post, IFormFile Image, int id)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    post.Image = ms.ToArray();
                }

                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.UserID = user.Id;
                _context.Add(post);
                _context.CompetitionPost.Add(new CompetitionPost { CompetitionID = id, PostID = post.ID, UserID = user.Id, SubmitDate = DateTime.Now });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAccount));
            }
            return new JsonResult(post);
            //return View(post);
        }

        public IActionResult Guide()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            var userPosts = _context.CompetitionPost.Include(a=>a.Competition).Include(b=>b.Post).Where(c=>c.UserID == user.Id).ToList();
            ViewData["User"] = user;
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
