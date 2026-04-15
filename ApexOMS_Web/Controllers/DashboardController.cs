using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApexDbContext _context;

        public DashboardController(ApexDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.Dashboards.OrderByDescending(d => d.shop_order_number).ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DashboardData data)
        {
            // The Guard: Block IB department from saving
            if (HttpContext.Session.GetString("UserRole") == "IB")
            {
                return Forbid(); // Or return Content("Access Denied");
            }
            if (ModelState.IsValid)
            {
                _context.Dashboards.Add(data);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}