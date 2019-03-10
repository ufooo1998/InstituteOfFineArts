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
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace InstituteOfFineArts.Controllers
{
    public class HomeController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;

        public HomeController(UserManager<CustomUser> userManager, InstituteOfFineArtsContext context, SignInManager<CustomUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

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
            ViewData["During"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.During).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["ComingUp"] = _context.Competition.Where(c => c.Status == CompetitonStatus.ComingUp).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["Examining"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Examining).OrderByDescending(c => c.CreatedAt).ToList();
            ViewData["Ended"] = _context.Competition.Include(a => a.CompetitionPosts).Where(c => c.Status == CompetitonStatus.Ended).OrderByDescending(c => c.CreatedAt).ToList();

            var competitionList = _context.Competition.OrderByDescending(a => a.CreatedAt).ToList();
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
            var competition = await _context.Competition.Include(a => a.User).FirstOrDefaultAsync(m => m.ID == id);
            var posts = _context.CompetitionPost.Include(a => a.Post).ThenInclude(b => b.User).Where(c => c.CompetitionID == id).ToList();
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
            EmailCofirm emailCofirm = new EmailCofirm();

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    post.Image = ms.ToArray();
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.UserID = user.Id;
               
                _context.Add(post);
                _context.CompetitionPost.Add(new CompetitionPost { CompetitionID = id, PostID = post.ID, UserID = user.Id, SubmitDate = DateTime.Now });
                emailCofirm.SendMail(user.Email, user.UserName, $"You posted an entry < a href = '{HtmlEncoder.Default.Encode(callbackUrl)}' > clicking here </ a >.");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAccount));
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

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyAccount()
        {
            var user = await GetCurrentUserAsync();
            var userPosts = _context.CompetitionPost.Include(a => a.Competition).Include(b => b.Post).Where(c => c.UserID == user.Id && c.Available == true).ToList();
            ViewData["User"] = user;
            return View(userPosts);
        }

        [Authorize(Roles = "Student")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditInfo()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            if (user.Status == AccountStatus.Inactivate)
            {
                return NotFound();
            }

            return View(user);
        }

        //Do edit task
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(string id, CustomUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customUser = await _userManager.FindByIdAsync(id);
                    if (customUser.Status == AccountStatus.Inactivate)
                    {
                        return NotFound();
                    }

                    if (user.DateOfBirth >= DateTime.Now.Date)
                    {
                        TempData["Faile"] = "Invalid Date of Birth!";
                        return View(customUser);
                    }
                    if (customUser.Email != user.Email)
                    {
                        var accountEmailExist = _context.Users.Where(a => a.Email == user.Email).Count();
                        if (accountEmailExist > 0)
                        {
                            TempData["Faile"] = "Email already taken!";
                            return View(customUser);
                        }
                    }

                    if (customUser.PhoneNumber != user.PhoneNumber)
                    {
                        var accountPhoneExist = _context.Users.Where(a => a.PhoneNumber == user.PhoneNumber && a.PhoneNumber != a.PhoneNumber).Count();
                        if (accountPhoneExist > 0)
                        {
                            TempData["Faile"] = "Phone number already taken!";
                            return View(customUser);
                        }
                    }

                    customUser.FullName = user.FullName;
                    customUser.Address = user.Address;
                    customUser.PhoneNumber = user.PhoneNumber;
                    customUser.DateOfBirth = user.DateOfBirth;
                    customUser.Gender = user.Gender;
                    customUser.Email = user.Email;
                    customUser.UpdatedAt = DateTime.Now;

                    var userRole = await _userManager.GetRolesAsync(customUser);
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
                return RedirectToAction(nameof(MyAccount));
            }
            return View(user);
        }

        [Authorize(Roles = "Student")]
        public IActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = _context.Post.Where(a => a.Available == true && a.ID == id).SingleOrDefault();
            return View(post);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(Post post, IFormFile Image)
        {
            //update post
            var editPost = _context.Post.Find(post.ID);
            if (editPost == null)
            {
                return NotFound();
            }
            editPost.PostName = post.PostName;
            editPost.Decription = post.Decription;
            using (var ms = new MemoryStream())
            {
                Image.CopyTo(ms);
                editPost.Image = ms.ToArray();
            }
            editPost.UpdatedAt = DateTime.Now;
            _context.Post.Update(editPost);
            _context.SaveChanges();

            TempData["Success"] = "Post updated!";
            return RedirectToAction(nameof(MyAccount));
        }

        [Authorize(Roles = "Student")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPost = _context.CompetitionPost.Include(a=>a.Competition).Include(b=>b.Post).Where(c=>c.PostID==id).SingleOrDefault();
            if (userPost == null)
            {
                return NotFound();
            }
            if (userPost.Available == false)
            {
                return NotFound();
            }

            return View(userPost);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = _context.CompetitionPost.Include(a=>a.Post).Where(b=>b.PostID == id).SingleOrDefault();
            post.Available = false;
            post.Post.Available = false;
            _context.Update(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Deleted success!";
            return RedirectToAction(nameof(MyAccount));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private Task<CustomUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private bool UserExist(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
