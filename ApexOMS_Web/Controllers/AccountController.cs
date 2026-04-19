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

        //[HttpPost]
        //public IActionResult Login(string txtUser, string txtPass)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.user_id == txtUser && u.user_pass == txtPass && u.active == 1);

        //    if (user != null)
        //    {
        //        // Store the Role in a Session variable
        //        HttpContext.Session.SetString("UserRole", user.Role ?? "IB");
        //        HttpContext.Session.SetString("UserName", user.user_name ?? "User");

        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View();
        //}
        // --- REGISTRATION ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                newUser.status = 0; // Waiting for Admin
                newUser.Role = "Pending";
                newUser.active = 1;

                _context.Users.Add(newUser);
                _context.SaveChanges();

                ViewBag.Message = "Registration successful! Please wait for Super Admin approval.";
                return View();
            }
            return View(newUser);
        }
        //[HttpPost]
        //public IActionResult Register(User newUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Set default values
        //        newUser.active = 1;

        //        _context.Users.Add(newUser);
        //        _context.SaveChanges();

        //        return RedirectToAction("Login");
        //    }
        //    return View(newUser);
        //}

        
        public IActionResult UserList()
        {
            // Security: Only SuperAdmin can see this
            if (HttpContext.Session.GetString("UserRole") != "SuperAdmin")
            { return Forbid(); }

            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult ApproveUser(int id, string role)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.status = 1; // Approve
                user.Role = role; // Assign Department
                _context.SaveChanges();
            }
            return RedirectToAction("UserList");
        }
        // --- LOGOUT ---

        public IActionResult Logout()
        {
            // For now, we just redirect to Login. 
            // Later, when we add Authentication, we will clear the cookie here.
            return RedirectToAction("Login");
        }
    }
}