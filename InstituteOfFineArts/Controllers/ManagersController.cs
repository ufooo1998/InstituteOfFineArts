using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstituteOfFineArts.Areas.Identity.Data;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Manager")]
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


        public async Task<IActionResult> Edit(string id, string URL)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRole = _context.UserRoles.Where(c => c.UserId == id).Single();
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name", currentRole.RoleId);
            return View(user);
        }
        public async Task<IActionResult> Details(string id)
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
        // Do edit task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CustomUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var existedit = _userManager.Users.SingleOrDefault(u=>u.Id == user.Id);
            if(existedit == null)
            {
                return NotFound();
            }
            existedit.PhoneNumber = user.PhoneNumber;
            existedit.DateOfBirth = user.DateOfBirth;
            existedit.Address = user.Address;
            _context.Users.Update(existedit);
            _context.SaveChanges();
            return new JsonResult(user);
        }

        public async Task<IActionResult> IndexCompe()
        {
            //var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            //return View(await instituteOfFineArtsContext.ToListAsync());
            return Redirect("/Competitions/Index");
        }
        public async Task<IActionResult> IndexPost()
        {
            //var instituteOfFineArtsContext = _context.Competition.Include(c => c.User);
            //return View(await instituteOfFineArtsContext.ToListAsync());
            return Redirect("/posts/Index");
        }
    }
}