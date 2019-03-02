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
    public class StaffsController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly InstituteOfFineArtsContext _context;
        public StaffsController(UserManager<CustomUser> userManager, InstituteOfFineArtsContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: Competitions
        public async Task<IActionResult> Index()
        {
            var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            return View(await instituteOfFineArtsContext.ToListAsync());
        }
        public async Task<IActionResult> CompetitionList()
        {
            var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            return View(await instituteOfFineArtsContext.ToListAsync());
        }


        // GET: Competitions/Details/5
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

        // GET: Competitions/Create
        public async Task<IActionResult> CreateCompetition()
        {
            var staffList = await _userManager.GetUsersInRoleAsync("staff");
            ViewData["UserID"] = new SelectList(staffList, "Id", "UserName");
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompetition([Bind("ID,CompetitionName,Decription,StartDate,EndDate,AwardDate,CreatedAt,UpdatedAt,Status,UserID")] Competition competition)
        {
            if (ModelState.IsValid)
            {
                competition.CreatedAt = DateTime.Now;
                competition.AwardDate = competition.EndDate.AddDays(2);
                if (competition.StartDate < DateTime.Now && DateTime.Now < competition.EndDate)
                {
                    competition.Status = CompetitonStatus.During;
                }
                if (DateTime.Now < competition.StartDate && DateTime.Now < competition.EndDate)
                {
                    competition.Status = CompetitonStatus.InComming;
                }
                if (competition.StartDate < DateTime.Now && competition.EndDate < DateTime.Now)
                {
                    competition.Status = CompetitonStatus.Ended;
                }
                _context.Add(competition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "UserName", competition.UserID);
            return View(competition);
        }

        // GET: Competitions/Edit/5
        public async Task<IActionResult> EditCompetition(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competition.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", competition.UserID);
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompetition(int id, [Bind("ID,CompetitionName,Decription,StartDate,EndDate,AwardDate,CreatedAt,UpdatedAt,Status,UserID")] Competition competition)
        {
            if (id != competition.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", competition.UserID);
            return View(competition);
        }

        // GET: Competitions/Delete/5
        public async Task<IActionResult> DeleteCompetition(int? id)
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

            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competition = await _context.Competition.FindAsync(id);
            _context.Competition.Remove(competition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitionExists(int id)
        {
            return _context.Competition.Any(e => e.ID == id);
        }

        public async Task<IActionResult> DetailsStudent(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDetail = await _context.Users.FindAsync(id);
            var accountRole = await _userManager.GetRolesAsync(accountDetail);
            ViewData["AccountRole"] = accountRole.Count;

            if (accountDetail == null)
            {
                return NotFound();
            }
            return View(accountDetail);
        }
        public async Task<IActionResult> DetailsPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        
    }
}