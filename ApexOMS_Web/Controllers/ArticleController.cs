using Microsoft.AspNetCore.Mvc;
using ApexOMS_Web.Data;
using ApexOMS_Web.Models;
using System.Linq;
using System;

namespace ApexOMS_Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApexDbContext _context;

        public ArticleController(ApexDbContext context)
        {
            _context = context;
        }

        // List View
        public IActionResult Index()
        {
            var data = _context.ArticleLasts.OrderByDescending(a => a.id).ToList();
            return View(data);
        }

        // Entry Form View
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save Logic
        [HttpPost]
        public IActionResult Create(ArticleLast article)
        {
            // The Guard: Block IB department from saving
            if (HttpContext.Session.GetString("UserRole") == "IB")
            {
                return Forbid(); // Or return Content("Access Denied");
            }
            // 1. Force ID to 0 so EF Core knows it's a new record
            article.id = 0;

            // 2. Set the date
            article.entry_date = DateTime.Now;

            try
            {
                _context.ArticleLasts.Add(article);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // This will show you the EXACT SQL error in the browser if it fails
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.Error = "Database Error: " + innerException;
                return View(article);
            }
        }
    }
}