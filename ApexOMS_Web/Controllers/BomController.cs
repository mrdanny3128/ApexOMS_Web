using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
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
            var data = _context.Boms.OrderByDescending(b => b.sl).Take(100).ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Bom bom)
        {
            // The Guard: Block IB department from saving
            if (HttpContext.Session.GetString("UserRole") == "IB")
            {
                return Forbid(); // Or return Content("Access Denied");
            }
            if (ModelState.IsValid)
            {
                bom.CREATEDATE = DateTime.Now;
                _context.Boms.Add(bom);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bom);
        }
    }
}