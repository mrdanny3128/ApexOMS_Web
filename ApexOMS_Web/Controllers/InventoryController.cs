using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data; // Your DbContext location
using ApexOMS_Web.Models;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApexDbContext _context;

        // Dependency Injection: The app gives the controller the DB connection automatically
        public InventoryController(ApexDbContext context)
        {
            _context = context;
        }

        // SHOW LIST
        public IActionResult Index()
        {
            // EF Core: SELECT * FROM tbl_invent_order
            var orders = _context.InventoryOrders.OrderByDescending(o => o.sl).ToList();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        // SAVE DATA (POST)
        [HttpPost]
        public IActionResult Create(InventoryOrder order)
        {
            // The Guard: Block IB department from saving
            if (HttpContext.Session.GetString("UserRole") == "IB")
            {
                return Forbid(); // Or return Content("Access Denied");
            }
            if (ModelState.IsValid)
            {
                // Generate Random ID if needed
                Random rnd = new Random();
                order.order_id = rnd.Next(100000, 999999);
                order.order_receive_date = DateTime.Now;
                order.order_status = 1;

                _context.InventoryOrders.Add(order); // Prepare the Insert
                _context.SaveChanges();              // Execute the Insert

                return RedirectToAction("Index");
            }
            return View(order);
        }

    }
}