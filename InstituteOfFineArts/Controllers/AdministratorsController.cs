using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class AdministratorsController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;
        private readonly UserManager<CustomUser> _userManager;
        public AdministratorsController(UserManager<CustomUser> userManager, InstituteOfFineArtsContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Define input model
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Input contain invalid characters")]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string Phone { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string RoleName { get; set; }
        }
        // Get list user of each role
        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> AccountList()
        {
            // get admin list
            var adminList = await _userManager.GetUsersInRoleAsync("Administrator");
            ViewData["AdminList"] = adminList.ToList();

            // get manager list
            var managerList = await _userManager.GetUsersInRoleAsync("Manager");
            ViewData["ManagerList"] = managerList.ToList();

            // get staff list
            var staffList = await _userManager.GetUsersInRoleAsync("Staff");
            ViewData["StaffList"] = staffList.ToList();

            // get student list
            var studentList = await _userManager.GetUsersInRoleAsync("Student");
            ViewData["StudentList"] = studentList.ToList();

            return View();
        }
        // Get data to the create form
        public IActionResult Create()
        {
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name");
            return View();
        }
        // Do create task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InputModel input)
        {
            if (ModelState.IsValid)
            {
                var user = new CustomUser {
                    UserName = input.UserName,
                    Email = input.Email,
                    Address = input.Address,
                    PhoneNumber = input.Phone,
                    DateOfBirth = input.DateOfBirth
                };
                var createUserResult = await _userManager.CreateAsync(user, input.Password);
                var createUserRoleResult = await _userManager.AddToRoleAsync(user, input.RoleName);
                if (createUserResult.Succeeded && createUserRoleResult.Succeeded)
                {
                    TempData["Success"] = "Created success.";
                    return RedirectToAction(nameof(AccountList));
                }
            }
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name");
            TempData["Fail"] = "Email already taken.";
            return View();
        }
        // Get detail data by id
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
        // Get data to the edit form
        public async Task<IActionResult> Edit(string id)
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
            var currentRole = _context.UserRoles.Where(c=>c.UserId == id).Single();
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name", currentRole.RoleId);
            return View(user);
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

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update<CustomUser>(user);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!UserExist(user.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}

            return new JsonResult(user);
        }
        private bool UserExist(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}