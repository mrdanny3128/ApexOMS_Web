using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class SampleOrderController : Controller
    {
        private readonly ApexDbContext _context;

        public SampleOrderController(ApexDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.SampleOrders.OrderByDescending(s => s.sl).Take(200).ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public IActionResult Create(SampleOrder order)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return Forbid();

            if (ModelState.IsValid)
            {
                order.entry_date = DateTime.Now;
                order.user_name = HttpContext.Session.GetString("UserName");
                _context.SampleOrders.Add(order);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }
    }
}