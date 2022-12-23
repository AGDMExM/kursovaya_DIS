using DIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DIS.Controllers
{
    public class UsersTableController : Controller
    {
        private ApplicationContext _context;

        public UsersTableController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        public IActionResult UsersTable(int? roleId, string fio, string passportSeries, string passportNumber)
        {
            ViewBag.NameUsersRole = _context.Users.Select(
                user => _context.Roles.First(role => role.Id == user.RoleId).Name).ToList();

            var roles = _context.Roles.ToList();
            roles.Insert(0, new Role() { Id = 0, Name = "Все" });
            ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roles, "Id", "Name");

            IQueryable<User> users = _context.Users;
            if (roleId != null && roleId != 0) // 0 is All
            {
                users = users.Where(u => u.RoleId == roleId);
            }
            if (!String.IsNullOrEmpty(fio))
            {
                users = users.Where(u => u.FIO.StartsWith(fio));
            }
            if (!String.IsNullOrEmpty(passportSeries) && Int32.TryParse(passportSeries, out _))
            {
                users = users.Where(u => u.PassportSeries.ToString().StartsWith(passportSeries));
            }
            if (!String.IsNullOrEmpty(passportNumber) && Int32.TryParse(passportNumber, out _))
            {
                users = users.Where(u => u.PassportNumber.ToString().StartsWith(passportNumber));
            }

            ViewBag.Users = users.ToList();

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateUser()
        {
            ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Roles, "Id", "Name");
            var selected = (ViewBag.Roles as SelectList).Where(x => x.Text == "user").First();
            selected.Selected = true;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("UsersTable");
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(int id)
        {
            User? user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("UsersTable");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditUser(int? id)
        {
            User? user = _context.Users.Find(id);
            if (user != null)
            {
                ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Roles, "Id", "Name");
                return View(user);
            }
            
            RedirectToAction("UsersTable");

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditUser(User user)
        {
            var _user = _context.Users.FirstOrDefault(x => x.Id == user.Id);

            if (_user != null)
            {
                _user.FIO = user.FIO;
                _user.Login = user.Login;
                _user.Password = user.Password;
                _user.PassportSeries = user.PassportSeries;
                _user.PassportNumber = user.PassportNumber;
                _user.RoleId = user.RoleId;
            }
            _context.SaveChanges();
            return RedirectToAction("UsersTable");
        }
    }
}
