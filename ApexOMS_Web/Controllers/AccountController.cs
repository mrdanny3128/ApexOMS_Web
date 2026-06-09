using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApexDbContext _context;

        public AccountController(ApexDbContext context)
        {
            _context = context;
        }

        // 1. ADD THIS METHOD - It handles the initial page load (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. This handles the button click (POST)

        [HttpPost]
        public IActionResult Login(string txtUser, string txtPass)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.user_id == txtUser &&
                u.user_pass == txtPass &&
                u.status == 1);

            if (user != null)
            {
                // FIX: You MUST set "UserID" here so the Profile page can find the record
                HttpContext.Session.SetString("UserID", user.user_id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.user_name);
                // Save the image path to session. Use a default if it's empty.
                string userImg = string.IsNullOrEmpty(user.image_path) ? "/images/profiles/default.png" : user.image_path;
                HttpContext.Session.SetString("UserProfilePic", userImg);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Account Pending Approval or Invalid Credentials.";
            return View();
        }


        // --- REGISTRATION ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User // This is your tbl_user entity
                {
                    user_id = model.user_id,
                    user_name = model.user_name,
                    user_email = model.user_email,
                    user_pass = model.user_pass, // Note: In a real app, hash this password!
                    Role = model.Role,
                    active = 1,
                    status = 0
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }



        public IActionResult UserList()
        {
            // Security: Only SuperAdmin can see this
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            { return Forbid(); }

            var users = _context.Users.ToList();
            return View(users);
        }


        [HttpPost]
        public IActionResult ApproveUser([FromBody] UserApprovalDto data)
        {
            if (data == null) return BadRequest();

            var user = _context.Users.Find(data.sl);
            if (user != null)
            {
                user.Role = data.Role;

                // Convert the string from JavaScript into the int the DB expects
                if (data.status == "Approved")
                {
                    user.status = 1;
                    user.active = 1;
                }
                else if (data.status == "Rejected")
                {
                    user.status = 2;
                    user.active = 0;
                }

                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string username, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            // FIX 1: Ensure '.Username' matches your actual Model property (e.g., .EmpID or .Email)
            var user = _context.Users.FirstOrDefault(u => u.user_id == username);

            if (user != null)
            {
                // FIX 2: Added the HashPassword method or used plain text
                user.user_pass = HashPassword(newPassword);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "User not found.");
            return View();
        }

        // FIX 2 Helper:
        private string HashPassword(string password) => password;

        // --- LOGOUT ---

        public IActionResult Logout()
        {
            // For now, we just redirect to Login. 
            // Later, when we add Authentication, we will clear the cookie here.
            return RedirectToAction("Login");
        }
        // --- PROFILE SECTION ---
        [HttpGet]
        public IActionResult Profile()
        {
            // Now this will work because "UserID" was set during Login
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User updatedUser, IFormFile? profilePic)
        {
            var sessionUserId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(sessionUserId)) return RedirectToAction("Login");

            var userInDb = _context.Users.FirstOrDefault(u => u.user_id == sessionUserId);

            if (userInDb != null)
            {
                // Update basic information
                userInDb.user_name = updatedUser.user_name;
                userInDb.user_email = updatedUser.user_email;

                // Only update password if the user actually typed a new one
                if (!string.IsNullOrEmpty(updatedUser.user_pass))
                {
                    userInDb.user_pass = updatedUser.user_pass;
                }

                // Handle Image Upload
                if (profilePic != null && profilePic.Length > 0)
                {
                    try
                    {
                        string folder = "images/profiles/";
                        string fileName = userInDb.user_id + Path.GetExtension(profilePic.FileName);
                        string serverFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

                        if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);

                        string filePath = Path.Combine(serverFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await profilePic.CopyToAsync(fileStream);
                        }

                        // FIX: Update the database column with the new image path
                        userInDb.image_path = "/" + folder + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Image upload failed: " + ex.Message;
                    }
                }

                _context.SaveChanges();

                // Update session so the name in the top-right corner refreshes immediately
                HttpContext.Session.SetString("UserName", userInDb.user_name);
                ViewBag.Message = "Profile updated successfully!";
            }

            return View(userInDb);
        }
    }
}
public class UserApprovalDto
{
    public int? sl { get; set; }
    public string? Role { get; set; }
    public string? status { get; set; }
}
