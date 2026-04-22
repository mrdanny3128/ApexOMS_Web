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

        [HttpGet]
        // 1. GET: Show the Edit Form
        public IActionResult Edit(int id)
        {
            var data = _context.Dashboards.Find(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(DashboardData updatedData)
        {
            // 1. Check if the ID is actually coming from the form
            if (updatedData.shop_order_number == 0)
            {
                return Content("Error: The ID was not passed correctly from the View.");
            }

            // 2. Clear validation for fields we aren't changing manually (like the ImageFile)
            ModelState.Remove("ImageFile");
            ModelState.Remove("shop_no_generated");

            // 3. Use the 'Attach' method - this is the strongest way to update
            _context.Dashboards.Attach(updatedData);
            _context.Entry(updatedData).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // 4. Handle the image path specifically
            if (updatedData.ImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(updatedData.ImageFile.FileName);
                string path = Path.Combine(wwwRootPath, "uploads", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    updatedData.ImageFile.CopyTo(fileStream);
                }
                updatedData.image_path = "/uploads/" + fileName;
            }
            else
            {
                // If no new image, tell EF to ignore this column so it doesn't overwrite with NULL
                _context.Entry(updatedData).Property(x => x.image_path).IsModified = false;
            }

            // 5. Tell EF never to try and update the auto-generated Shop Order No
            _context.Entry(updatedData).Property(x => x.shop_no_generated).IsModified = false;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}