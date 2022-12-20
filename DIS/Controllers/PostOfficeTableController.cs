using DIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DIS.Controllers
{
    public class PostOfficeTableController : Controller
    {
        private ApplicationContext _context;

        public PostOfficeTableController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult PostOfficeTable(string address, string region, string city)
        {
            ViewBag.PostOffices = _context.PostOffices.ToList();
             
            var regions = _context.PostOffices.Select(x => x.Region).ToList();
            regions.Insert(0, "Все");
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(regions);

            var citys = _context.PostOffices.Select(x => x.City).ToList();
            citys.Insert(0, "Все");
            ViewBag.Citys = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(citys);

            IQueryable<PostOffice> postOffices = _context.PostOffices;
            if (!String.IsNullOrEmpty(region) && region != "Все")
            {
                postOffices = postOffices.Where(x => x.Region == region);
            }
            if (!String.IsNullOrEmpty(city) && city != "Все")
            {
                postOffices = postOffices.Where(x => x.City == city);
            }
            if (!String.IsNullOrEmpty(address))
            {
                postOffices = postOffices.Where(x => x.Address.StartsWith(address));
            }

            ViewBag.PostOffices = postOffices.ToList();

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreatePostOffice()
        {
            var regions = _context.PostOffices.Select(x => x.Region).ToList();
            regions.Insert(0, "Все");
            ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(regions);
            var selectedRegion = (ViewBag.Regions as SelectList).First();
            selectedRegion.Selected = true;

            var citys = _context.PostOffices.Select(x => x.City).ToList();
            citys.Insert(0, "Все");
            ViewBag.Citys = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(citys);
            var selectedCity = (ViewBag.Citys as SelectList).First();
            selectedCity.Selected = true;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreatePostOffice(PostOffice postOffice)
        {
            _context.PostOffices.Add(postOffice);
            _context.SaveChanges();

            return RedirectToAction("PostOfficeTable");
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeletePostOffice(int id)
        {
            PostOffice? postOffice = _context.PostOffices.Find(id);
            if (postOffice != null)
            {
                _context.PostOffices.Remove(postOffice);
                _context.SaveChanges();
            }

            return RedirectToAction("PostOfficeTable");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditPostOffice(int? id)
        {
            PostOffice? postOffice = _context.PostOffices.Find(id);
            if (postOffice != null)
            {
                var regions = _context.PostOffices.Select(x => x.Region).ToList();
                regions.Insert(0, "Все");
                ViewBag.Regions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(regions);
                var selectedRegion = (ViewBag.Regions as SelectList).First();
                selectedRegion.Selected = true;

                var citys = _context.PostOffices.Select(x => x.City).ToList();
                citys.Insert(0, "Все");
                ViewBag.Citys = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(citys);
                var selectedCity = (ViewBag.Citys as SelectList).First();
                selectedCity.Selected = true;

                return View(postOffice);
            }

            RedirectToAction("PostOfficeTable");

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditPostOffice(PostOffice postOffice)
        {
            var editedPostOffice = _context.PostOffices.First(x => x.Id == postOffice.Id);
            editedPostOffice.Region = postOffice.Region;
            editedPostOffice.City = postOffice.City;
            editedPostOffice.Address = postOffice.Address;
            _context.SaveChanges();
            return RedirectToAction("PostOfficeTable");
        }

/*        public IActionResult Index()
        {
            return View();
        }*/
    }
}
