using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstituteOfFineArts.Areas.Identity.Data;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
{
    public class ManagersController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;
        private readonly UserManager<CustomUser> _userManager;
        public ManagersController(UserManager<CustomUser> userManager, InstituteOfFineArtsContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> AccountList()
        {
            
            var staffList = await _userManager.GetUsersInRoleAsync("Staff");
            ViewData["StaffList"] = staffList.ToList();

            // get student list
            var studentList = await _userManager.GetUsersInRoleAsync("Student");
            ViewData["StudentList"] = studentList.ToList();

            return View();
        }
        
        public async Task<IActionResult> IndexCompe()
        {
            //var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            //return View(await instituteOfFineArtsContext.ToListAsync());
            return Redirect("/Competitions/Index");
        }
        public async Task<IActionResult> CompetitionList()
        {
            UpdateStatus();
            var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            return View(await instituteOfFineArtsContext.ToListAsync());
            
        }

        public async Task<IActionResult> DetailsCompetition(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competition
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (competition == null)
            {
                return NotFound();
            }

            // Get list post of competition
            var competitionPosts = _context.CompetitionPost.Where(c => c.CompetitionID == id).Include(c => c.Post).ThenInclude(c => c.User).ToList();
            ViewData["PostList"] = competitionPosts;

            return View(competition);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var accountDetail = _context.Users.Find(id);
            ViewData["PostUser"] = _context.CompetitionPost
                .Include(a => a.Competition)
                .Include(b => b.Post)
                .Where(d => d.UserID == id).ToList();
            var checkrole = _context.UserRoles.SingleOrDefault(m=>m.UserId == id);
            var rolename = _context.Roles.Find(checkrole.RoleId);
            ViewData["Checkrole"] = rolename.Name;
            //var postuser = _context.CompetitionPost
            //    .Include(p => p.Post)
            //    .ThenInclude(u => u.User)
            //    .Where(m=>m.UserID == id).ToList();
            //var accountRole = await _userManager.GetRolesAsync(accountDetail);
            //ViewData["AccountRole"] = accountRole.Count;
            //ViewData["ListPost"] = postuser;
            if (accountDetail == null)
            {
                return NotFound();
            }
            return View(accountDetail);
        }
        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            ViewData["Role"] = _context.Roles.Find(_context.UserRoles.Where(c => c.UserId == user.Id).Single().RoleId);
            return View(user);
        }
        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private void UpdateStatus()
        {
            var competitionList = _context.Competition.ToList();
            for (int i = 0; i < competitionList.Count; i++)
            {
                if (competitionList[i].StartDate <= DateTime.Now.Date && DateTime.Now.Date <= competitionList[i].EndDate)
                {
                    competitionList[i].Status = CompetitonStatus.During;
                }
                else
                {
                    if (DateTime.Now.Date < competitionList[i].StartDate && DateTime.Now.Date < competitionList[i].EndDate)
                    {
                        competitionList[i].Status = CompetitonStatus.ComingUp;
                    }
                    else
                    {
                        if (competitionList[i].StartDate.Date < DateTime.Now.Date && competitionList[i].EndDate.Date < DateTime.Now.Date)
                        {
                            competitionList[i].Status = CompetitonStatus.Ended;
                        }
                        if (competitionList[i].EndDate.Date < DateTime.Now.Date && DateTime.Now.Date <= competitionList[i].AwardDate.Date)
                        {
                            competitionList[i].Status = CompetitonStatus.Examining;
                        }
                    }
                }
                _context.Update(competitionList[i]);
                _context.SaveChanges();
            }
        }

    }
}