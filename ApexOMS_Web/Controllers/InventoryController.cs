using ApexOMS_Web.Data; // Your DbContext location
using ApexOMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return RedirectToAction("Index");

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

            ModelState.Remove("order_id");
            ModelState.Remove("order_receive_date");

            if (ModelState.IsValid)
            {
                // Generate sequential Order ID
                var lastOrder = _context.InventoryOrders.OrderByDescending(o => o.order_id).FirstOrDefault();
                int nextId = (lastOrder != null && lastOrder.order_id.HasValue) ? lastOrder.order_id.Value + 1 : 100001;
                
                order.order_id = nextId;
                order.order_receive_date = DateTime.Now;

                _context.InventoryOrders.Add(order); // Prepare the Insert
                _context.SaveChanges();              // Execute the Insert

                return RedirectToAction("Index");
            }
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int sl)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return RedirectToAction("Index");

            // We use 'id' as the parameter to match your URL: ?id=12
            var order = await _context.InventoryOrders
                .FirstOrDefaultAsync(m => m.sl == sl);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // 3. POST: /Inventory/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryOrder model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryOrderExists(model.sl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int sl)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "IB") return Content("Access Denied: View-Only Department.");

            var order = await _context.InventoryOrders.FirstOrDefaultAsync(m => m.sl == sl);
            if (order != null)
            {
                _context.InventoryOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryOrderExists(int sl)
        {
            return _context.InventoryOrders.Any(e => e.sl == sl);
        }
    }
}

    
