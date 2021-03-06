﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using InstituteOfFineArts.Areas.Identity.Data;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Staff")]
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
        public IActionResult Index()
        {
            UpdateStatus();
            ViewData["CompetitionCount"] = _context.Competition.ToList().Count;
            ViewData["PostCount"] = _context.Post.ToList().Count;
            ViewData["MarkCount"] = _context.Competition.Where(c => c.Status == CompetitonStatus.Examining).ToList().Count;

            return View();
        }

        public async Task<IActionResult> CompetitionList()
        {
            UpdateStatus();
            var instituteOfFineArtsContext = _context.Competition.Include(c => c.User).Where(a => a.Available == true).OrderByDescending(b=>b.CreatedAt);
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
           
            var competitionPosts = _context.CompetitionPost.Where(c => c.CompetitionID == id).Include(c => c.Post).ThenInclude(c => c.User).ToList();
            ViewData["PostList"] = competitionPosts;

            return View(competition);
        }

        // GET: Competitions/Create
        public async Task<IActionResult> CreateCompetition()
        {
            var staffList = await _userManager.GetUsersInRoleAsync("staff");
            ViewData["UserID"] = new SelectList(staffList.Where(a => a.Status == AccountStatus.Activate), "Id", "FullName");
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompetition(Competition competition)
        {
            if (ModelState.IsValid)
            {
                EmailCofirm emailCofirm = new EmailCofirm();
                if (competition.StartDate < competition.EndDate)
                {
                    var user = _userManager.Users.SingleOrDefault(u => u.Id == competition.UserID);
                    competition.CreatedAt = DateTime.Now;
                    competition.UpdatedAt = DateTime.Now;
                    competition.Available = true;
                    competition.AwardDate = competition.EndDate.AddDays(2);
                    competition.StartDate = competition.StartDate.Date;
                    CheckStatus(competition);
                    _context.Add(competition);

                    emailCofirm.SendMail(user.Email, user.UserName, $"You are selected as a judge for a competition < a href = '{HtmlEncoder.Default.Encode("https://localhost:44312/Staffs/DetailsCompetition/" + competition.ID)}' > clicking here </ a >.");

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(CompetitionList));
                }
                TempData["Error"] = "Invalid start date & end date!";
            }

            var staffList = await _userManager.GetUsersInRoleAsync("staff");
            ViewData["UserID"] = new SelectList(staffList, "Id", "FullName", competition.UserID);
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
            var staffList = await _userManager.GetUsersInRoleAsync("staff");
            ViewData["UserID"] = new SelectList(staffList, "Id", "FullName", competition.UserID);
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompetition(int id, Competition competition)
        {
            if (id != competition.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    competition.UpdatedAt = DateTime.Now;
                    competition.AwardDate = competition.EndDate.AddDays(2);
                    CheckStatus(competition);
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
                return RedirectToAction(nameof(CompetitionList));
            }
            var staffList = await _userManager.GetUsersInRoleAsync("staff");
            ViewData["UserID"] = new SelectList(staffList, "Id", "FullName", competition.UserID);
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
            ViewData["PostCount"] = _context.CompetitionPost.Where(c => c.CompetitionID == competition.ID).ToList();
            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competition = await _context.Competition.FindAsync(id);
            competition.Available = false;           
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CompetitionList));
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
            var getRoleId = _context.UserRoles.Where(c => c.UserId == id).Single().RoleId;
            ViewData["Role"] = _context.Roles.Find(getRoleId).Name;
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

        public IActionResult ExaminingCompetition()
        {
            UpdateStatus();
            var examiningCompetition = _context.Competition.Where(a => a.Available == true && a.Status == CompetitonStatus.Examining).ToList();
            return View(examiningCompetition);
        }

        public async Task<IActionResult> ExaminingPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var competition = _context.Competition.Find(id);
            if (competition == null || competition.Available == false || competition.Status != CompetitonStatus.Examining)
            {
                return NotFound();
            }

            var competitionPosts = _context.CompetitionPost.Where(c => c.CompetitionID == id).Include(c => c.Post).ThenInclude(c => c.User).ToList();
            var user = await GetCurrentUserAsync();
            ViewData["PostList"] = competitionPosts;
            return View(user);
        }

        public async Task<IActionResult> MarkPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitionPost = _context.CompetitionPost.Where(c => c.PostID == id).Single();
            ViewData["CompetitionId"] = competitionPost.CompetitionID;
            var post = await _context.Post
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        public async Task<IActionResult> Mark(int competitionId, int? Id, int PostPoint)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var post = _context.CompetitionPost.Where(c => c.PostID == Id).Single();
            var user = await GetCurrentUserAsync();
            if (post.StaffSubmit != null)
            {
                var obj = JsonConvert.DeserializeObject<List<StaffSubmitList>>(post.StaffSubmit);
                obj.Add(new StaffSubmitList { StaffId = user.Id, Point = PostPoint });
                var staffSubmit = JsonConvert.SerializeObject(obj, Formatting.Indented);
                post.StaffSubmit = staffSubmit;
            }
            else
            {
                List<StaffSubmitList> obj = new List<StaffSubmitList>();
                obj.Add(new StaffSubmitList { StaffId = user.Id, Point = PostPoint });
                var staffSubmit = JsonConvert.SerializeObject(obj, Formatting.Indented);
                post.StaffSubmit = staffSubmit;
            }
            post.PostPoint += PostPoint;
            _context.CompetitionPost.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("ExaminingPost/" + competitionId);
        }

        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            ViewData["Role"] = _context.Roles.Find(_context.UserRoles.Where(c => c.UserId == user.Id).Single().RoleId);
            return View(user);
        }

        // get current user login
        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        //check status of competition
        private void CheckStatus(Competition competition)
        {
            if (competition.StartDate <= DateTime.Now.Date && DateTime.Now.Date <= competition.EndDate)
            {
                competition.Status = CompetitonStatus.Ongoing;
            }
            else
            {
                if (DateTime.Now.Date < competition.StartDate && DateTime.Now.Date < competition.EndDate)
                {
                    competition.Status = CompetitonStatus.ComingUp;
                }
                else
                {
                    if (competition.StartDate.Date < DateTime.Now.Date && competition.EndDate.Date < DateTime.Now.Date)
                    {
                        competition.Status = CompetitonStatus.Ended;
                    }
                    else
                    {
                        if (competition.EndDate.Date < DateTime.Now.Date && DateTime.Now.Date <= competition.AwardDate.Date)
                        {
                            competition.Status = CompetitonStatus.Examining;
                        }
                        if (competition.EndDate.Date < DateTime.Now.Date && DateTime.Now.Date <= competition.AwardDate.Date)
                        {
                            competition.Status = CompetitonStatus.Examining;
                        }
                    }
                }
            }
        }

        //update status of competition
        private void UpdateStatus()
        {
            var competitionList = _context.Competition.ToList();
            for (int i = 0; i < competitionList.Count; i++)
            {
                if (competitionList[i].StartDate <= DateTime.Now.Date && DateTime.Now.Date <= competitionList[i].EndDate)
                {
                    competitionList[i].Status = CompetitonStatus.Ongoing;
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