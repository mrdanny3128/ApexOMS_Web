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
                u.status == 1); // Check for approval

            if (user != null)
            {
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.user_name);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Account pending approval or invalid credentials.";
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

    }
}
public class UserApprovalDto
{
    public int? sl { get; set; }
    public string? Role { get; set; }
    public string? status { get; set; }
}