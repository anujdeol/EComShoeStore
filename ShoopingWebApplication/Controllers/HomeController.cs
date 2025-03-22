using Microsoft.AspNetCore.Mvc;
using ShoopingWebApplication.Data;
using ShoopingWebApplication.Models;
using System.Diagnostics;
using System.Linq;

namespace ShoopingWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            return View(categories);
        } 

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(string fullName, string email, string message)
        {
            if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(message))
            {
                
                ViewBag.FullName = fullName;
                ViewBag.Email = email;
                ViewBag.Message = message;

                return View("ContactConfirmation");
            }

          
            ViewBag.ErrorMessage = "Please fill in all the fields.";
            return View("Contact");
        }
    

public IActionResult FAQ()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
