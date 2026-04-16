using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class BomController : Controller
    {
        private readonly ApexDbContext _context;

        public BomController(ApexDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Boms.OrderByDescending(b => b.sl).ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            // Block IB from even seeing the empty form
            if (role == "IB") return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Bom bom)
        {
            var role = HttpContext.Session.GetString("UserRole");
            // Security Lock: Only SuperAdmin and DesignLab allowed
            if (role == "IB") return Content("Access Denied: View-Only Department.");

            if (ModelState.IsValid)
            {
                bom.CREATEDATE = DateTime.Now;
                bom.user_id = HttpContext.Session.GetString("UserName");

                _context.Boms.Add(bom);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bom);
        }
    }
}