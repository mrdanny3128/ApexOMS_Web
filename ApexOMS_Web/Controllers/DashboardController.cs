using ApexOMS_Web.Data;
using Microsoft.AspNetCore.Hosting;
using ApexOMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace ApexOMS_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApexDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DashboardController(ApexDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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
           // ModelState.Remove("shop_no_generated");
            if (ModelState.IsValid)
            {
                // HANDLE IMAGE UPLOAD
                if (data.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath; // Use IWebHostEnvironment
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "uploads", fileName);

                    // 2. Save file to physical folder
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        data.ImageFile.CopyTo(fileStream);
                    }

                    // 3. Save relative path to DB (This is what the <img> tag uses)
                    data.image_path = "/uploads/" + fileName;
                }
                //var lastEntry = _context.Dashboards.OrderByDescending(d => d.shop_order_number).FirstOrDefault();
                //int nextId = (lastEntry != null) ? lastEntry.shop_order_number + 1 : 1;

                // 3. Set the formatted string (APX-001, APX-002, etc.)
                //data.shop_no_generated = "APX-" + nextId.ToString("D3");
                _context.Dashboards.Add(data);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}