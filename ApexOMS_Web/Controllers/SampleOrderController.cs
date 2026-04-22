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
        private readonly IWebHostEnvironment _hostEnvironment;

        public SampleOrderController(ApexDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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
        [ValidateAntiForgeryToken] // Recommended for security
        public IActionResult Create(SampleOrder data)
        {
            // 1. Tell the server not to worry about these specific fields
            ModelState.Remove("ImageFile");
            ModelState.Remove("image_path");
            // If your primary key 'sl' is auto-increment, remove it too
            ModelState.Remove("sl");
            // Important: Re-add validation if you removed it
            if (ModelState.IsValid)
            {
                // HANDLE IMAGE UPLOAD
                if (data.ImageFile != null && data.ImageFile.Length > 0)
                {
                    // 1. Path to wwwroot/uploads
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    if (string.IsNullOrEmpty(wwwRootPath)) return Content("Error: HostEnvironment is NULL.");

                    // 2. Create unique filename
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "uploads", fileName);

                    // 3. Ensure folder exists
                    string uploadsFolder = Path.Combine(wwwRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    // 4. Save file to physical folder
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        data.ImageFile.CopyTo(fileStream);
                    }

                    // 5. Save relative path to DB (This is what the <img> tag uses)
                    data.image_path = "/uploads/" + fileName;
                }

                _context.SampleOrders.Add(data);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            // 2. If it reaches here, it FAILED. 
            // This part helps you see WHAT went wrong in the "Output" window
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }
            // If validation fails, return to view with data
            return View(data);
        }
        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = _context.SampleOrders.Find(id);
            if (data == null) return NotFound();
            return View(data);
        }

        // POST: Edit
        [HttpPost]
        public IActionResult Edit(SampleOrder updatedData)
        {
            // 1. Force the ID and ImagePath to be tracked
            _context.SampleOrders.Attach(updatedData);
            _context.Entry(updatedData).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

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
                // Keep existing image if no new file is uploaded
                _context.Entry(updatedData).Property(x => x.image_path).IsModified = false;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    
}

}