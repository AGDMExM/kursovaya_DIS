using DIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace DIS.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;

        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Index()
        {
            return RedirectToAction("PackageTable", "PackageTable");
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult Info()
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