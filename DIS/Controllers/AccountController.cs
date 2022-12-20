using DIS.Models;
using DIS.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;

namespace DIS.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;

        public AccountController(ApplicationContext context)
        {
            _context = context; 
        }

        //[HttpGet]
        //[Authorize(Roles = "admin, user")]
        public IActionResult Account()
        {
            var login = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Login == login);
            ViewBag.User = user;
            ViewBag.NameRole = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId).Name;

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                
                if (user == null)
                {
                    user = new User 
                    { 
                        FIO = model.FIO,
                        PassportSeries = model.PassportSeries,
                        PassportNumber = model.PassportNumber,
                        Login = model.Login, 
                        Password = model.Password };
                    Role? userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Account", "Account");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Account", "Account");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");

                ViewBag.User = user;
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        //[Authorize(Roles = "admin")]
        public ActionResult EditUser(int? id)
        {
            User? user = _context.Users.Find(id);
            if (user != null)
            {
                ViewBag.Roles = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Roles, "Id", "Name");
                return View(user);
            }

            RedirectToAction("Account");

            return NotFound();
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public ActionResult EditUser(User user)
        {
            var editedUser = _context.Users.First(x => x.Id == user.Id);
            editedUser.FIO = user.FIO;
            editedUser.Login = user.Login;
            editedUser.PassportNumber = user.PassportNumber;
            editedUser.PassportSeries = user.PassportSeries;
            editedUser.Password = user.Password;
            editedUser.RoleId = user.RoleId;
            _context.SaveChanges();
            return RedirectToAction("Account");
        }

        /*public IActionResult Index()
        {
            return View();
        }*/
    }
}
