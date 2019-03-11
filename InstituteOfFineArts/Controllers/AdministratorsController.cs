using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InstituteOfFineArts.Areas.Identity.Data;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InstituteOfFineArts.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorsController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly ILogger<AdministratorsController> _logger;
        public AdministratorsController(ILogger<AdministratorsController> logger, UserManager<CustomUser> userManager, InstituteOfFineArtsContext context, SignInManager<CustomUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        // Define input model
        public InputModel Input { get; set; }
        public InputModel2 Input2 { get; set; }
        public class InputModel2
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public class InputModel
        {
            [Required]
            [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Input invalid")]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

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
            public Gender Gender { get; set; }

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
        public async Task<IActionResult> Index()
        {

            var Administrators = await _userManager.GetUsersInRoleAsync("Administrator");
            ViewData["Administrators"] = Administrators.Where(a=>a.Status == AccountStatus.Activate).Count();

            var managers = await _userManager.GetUsersInRoleAsync("Manager");
            ViewData["Managers"] = managers.Where(a => a.Status == AccountStatus.Activate).Count();

            var staffs = await _userManager.GetUsersInRoleAsync("Staff");
            ViewData["Staffs"] = staffs.Where(a => a.Status == AccountStatus.Activate).Count();

            var students = await _userManager.GetUsersInRoleAsync("Student");
            ViewData["Students"] = students.Where(a => a.Status == AccountStatus.Activate).Count();

            return View();
        }
        public async Task<IActionResult> AccountList()
        {

            // get admin list
            var adminList = await _userManager.GetUsersInRoleAsync("Administrator");
            ViewData["AdminList"] = adminList.Where(a => a.Status == AccountStatus.Activate).ToList();

            // get manager list
            var managerList = await _userManager.GetUsersInRoleAsync("Manager");
            ViewData["ManagerList"] = managerList.Where(a => a.Status == AccountStatus.Activate).ToList();

            // get staff list
            var staffList = await _userManager.GetUsersInRoleAsync("Staff");
            ViewData["StaffList"] = staffList.Where(a => a.Status == AccountStatus.Activate).ToList();

            // get student list
            var studentList = await _userManager.GetUsersInRoleAsync("Student");
            ViewData["StudentList"] = studentList.Where(a => a.Status == AccountStatus.Activate).ToList();

            return View();
        }
        // Get data to the create form
        public IActionResult Create()
        {
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
            return View();
        }
        // Do create task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InputModel input)
        {
            if (ModelState.IsValid)
            {
                if (input.DateOfBirth >= DateTime.Now.Date)
                {
                    ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
                    TempData["Faile"] = "Invalid Date of Birth!";
                    return View();
                }
                var accountNameExist = _context.Users.Where(a => a.UserName == input.UserName).Count();
                if (accountNameExist > 0)
                {
                    ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
                    TempData["Faile"] = "Account name already taken!";
                    return View();
                }
                var accountEmailExist = _context.Users.Where(a => a.Email == input.Email).Count();
                if (accountEmailExist > 0)
                {
                    ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
                    TempData["Faile"] = "Email already taken!";
                    return View();
                }
                var accountPhoneExist = _context.Users.Where(a => a.PhoneNumber == input.Phone).Count();
                if (accountPhoneExist > 0)
                {
                    ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
                    TempData["Faile"] = "Phone number already taken!";
                    return View();
                }
                var user = new CustomUser {
                    FullName = input.FullName,
                    UserName = input.UserName,
                    Email = input.Email,
                    Gender = input.Gender,
                    Address = input.Address,
                    PhoneNumber = input.Phone,
                    DateOfBirth = input.DateOfBirth,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = AccountStatus.Activate
                };
                var createUserResult = await _userManager.CreateAsync(user, input.Password);
                var createUserRoleResult = await _userManager.AddToRoleAsync(user, input.RoleName);
                if (createUserResult.Succeeded && createUserRoleResult.Succeeded)
                {
                    TempData["Success"] = "Created success.";
                    return RedirectToAction(nameof(AccountList));
                }
            }
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", "Student");
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
            if (accountDetail == null)
            {
                return NotFound();
            }
            if (accountDetail.Status == AccountStatus.Inactivate)
            {
                return NotFound();
            }

            var accountRole = await _userManager.GetRolesAsync(accountDetail);
            ViewData["AccountRole"] = accountRole;
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
            if (user.Status == AccountStatus.Inactivate)
            {
                return NotFound();
            }

            var currentRole = await _userManager.GetRolesAsync(user);
            ViewData["RoleList"] = new SelectList(_context.Roles, "Name", "Name", currentRole[0]);
            return View(user);
        }
        // Do edit task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CustomUser user, string role)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var customUser = await _userManager.FindByIdAsync(id);
                    if (customUser.Status == AccountStatus.Inactivate)
                    {
                        return NotFound();
                    }
                    customUser.FullName = user.FullName;
                    customUser.Address = user.Address;
                    customUser.PhoneNumber = user.PhoneNumber;
                    customUser.DateOfBirth = user.DateOfBirth;
                    customUser.Gender = user.Gender;
                    customUser.Email = user.Email;

                    var userRole = await _userManager.GetRolesAsync(customUser);
                    await _userManager.RemoveFromRoleAsync(customUser, userRole[0]);
                    await _userManager.AddToRoleAsync(customUser, role);
                    await _userManager.UpdateAsync(customUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExist(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Edited success!";
                return RedirectToAction(nameof(AccountList));
            }
            return View(user);
        }

        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            ViewData["Role"] = _context.Roles.Find(_context.UserRoles.Where(c => c.UserId == user.Id).Single().RoleId);
            return View(user);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(InputModel2 input2)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, input2.OldPassword, input2.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    TempData["Faile"] = "Current password is not correct!";
                    return View();
                }
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password has changed!";
                return RedirectToAction(nameof(MyAccount));
            }
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDetail = await _context.Users.FindAsync(id);
            if (accountDetail.Status == AccountStatus.Inactivate)
            {
                return NotFound();
            }
            var accountRole = await _userManager.GetRolesAsync(accountDetail);
            ViewData["AccountRole"] = accountRole;
            if (accountDetail == null)
            {
                return NotFound();
            }
            return View(accountDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Status = AccountStatus.Inactivate;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Deleted success!";
            return RedirectToAction(nameof(AccountList));
        }

        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private bool UserExist(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}