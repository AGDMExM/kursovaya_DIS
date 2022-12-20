using DIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace DIS.Controllers
{
    public class PackageTableController : Controller
    {
        private ApplicationContext _context;
        private readonly List<string> STATUS_LIST = new List<string>()
        { 
            "Готовится к отправке", 
            "Отправлено", 
            "Прибыло в пункт выдачи" 
        };

        public PackageTableController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult PackageTable(
            string name, 
            string status,
            string sender,
            string recipient,
            string senderPostOffice,
            string recieptPostOffice)
        {
            string? login = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            User user = _context.Users.First(u => u.Login == login);

            ViewBag.Packages = _context.Packages.ToList();
            ViewBag.UsersId = _context.Users.Select(x => x.Id).ToList();
            ViewBag.OfficesId = _context.PostOffices.Select(x => x.Id).ToList();

            ViewBag.UserNames = _context.Users.Select(x => x.FIO).ToList();
            ViewBag.OfficeNames = _context.PostOffices.Select(x => x.Address).ToList();

            var names = _context.Packages.
                Select(x => x.Name).
                ToList();
            names.Insert(0, "Все");
            var statusList = STATUS_LIST;
            statusList.Insert(0, "Все");
            var senders = _context.Users.
                Where(x => _context.Packages.
                    Select(x => x.IdSender).
                    Contains(x.Id)).
                Select(x => x.FIO).
                ToList();
            senders.Insert(0, "Все");
            var recipients = _context.Users.
                Where(x => _context.Packages.
                    Select(x => x.IdRecipient).
                    Contains(x.Id)).
                Select(x => x.FIO).
                ToList();
            recipients.Insert(0, "Все");
            var sendersPostOffice = _context.PostOffices.
                Where(x => _context.Packages.
                    Select(x => x.IdSenderPostOffice).
                    Contains(x.Id)).
                Select(x => x.Address).
                ToList();
            sendersPostOffice.Insert(0, "Все");
            var recieptsPostOffice = _context.PostOffices.
                Where(x => _context.Packages.
                    Select(x => x.IdRecipientPostOffice).
                    Contains(x.Id)).
                Select(x => x.Address).
                ToList();
            recieptsPostOffice.Insert(0, "Все");

            ViewBag.NamesSelectList = new SelectList(names);
            ViewBag.StatusSelectList = new SelectList(statusList);
            ViewBag.SendersSelectList = new SelectList(senders);
            ViewBag.RecipientsSelectList = new SelectList(recipients);
            ViewBag.SenderPostOfficeSelectList = new SelectList(sendersPostOffice);
            ViewBag.RecieptPostOfficeSelectList = new SelectList(recieptsPostOffice);

            ViewBag.Names = names.ToList();
            ViewBag.Status = statusList.ToList();
            ViewBag.Senders = senders.ToList();
            ViewBag.Recipients = recipients.ToList();
            ViewBag.SenderPostOffice = sendersPostOffice.ToList();
            ViewBag.RecieptPostOffice = recieptsPostOffice.ToList();

            IQueryable<Package> packages = _context.Packages;
            if (!String.IsNullOrEmpty(name) && name != "Все")
            {
                packages = packages.Where(u => u.Name.StartsWith(name));
            }
            if(!String.IsNullOrEmpty(status) && status != "Все")
            {
                packages = packages.Where(u => u.Status.StartsWith(status));
            }
            if (user.RoleId != 1)
            {
                packages = packages.Where(x => x.IdSender == user.Id || x.IdRecipient == user.Id);
            }else
            {
                if (!String.IsNullOrEmpty(sender) && sender != "Все")
                {
                    packages = packages.Where(u => u.IdSender == _context.Users.First(x => x.FIO == sender).Id);
                }
            }
            if (!String.IsNullOrEmpty(recipient) && recipient != "Все")
            {
                packages = packages.Where(u => u.IdRecipient == _context.Users.First(x => x.FIO == recipient).Id);
            }
            if (!String.IsNullOrEmpty(senderPostOffice) && senderPostOffice != "Все")
            {
                packages = packages.Where(u => u.IdSenderPostOffice == _context.PostOffices.First(x => x.Address == senderPostOffice).Id);
            }
            if (!String.IsNullOrEmpty(recieptPostOffice) && recieptPostOffice != "Все")
            {
                packages = packages.Where(u => u.IdRecipientPostOffice == _context.PostOffices.First(x => x.Address == recieptPostOffice).Id);
            }

            ViewBag.Packages = packages.ToList();

            return View();
        }

        [HttpGet]
        public ActionResult CreatePackage()
        {
            string? login = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            User user = _context.Users.First(u => u.Login == login);

            if (user.RoleId != 1)
                ViewBag.Status = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new List<string>() { "Готовится к отправке" });
            else
                ViewBag.Status = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(STATUS_LIST);
            var selectedStatus = (ViewBag.Status as SelectList).First();
            selectedStatus.Selected = true;

            if (user.RoleId != 1)
                ViewBag.Senders = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users.Where(x => x.Id == user.Id), "Id", "FIO");
            else
                ViewBag.Senders = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "Id", "FIO");
            var selectedSender = (ViewBag.Senders as SelectList).First();
            selectedSender.Selected = true;

            ViewBag.Recipients = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "Id", "FIO");
            var selectedRecipient = (ViewBag.Recipients as SelectList).First();
            selectedRecipient.Selected = true;

            ViewBag.SenderPostOffice = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.PostOffices, "Id", "Address");
            var selectedSenderPostOffice = (ViewBag.SenderPostOffice as SelectList).First();
            selectedSenderPostOffice.Selected = true;

            ViewBag.RecieptPostOffice = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.PostOffices, "Id", "Address");
            var selectedRecieptPostOffice = (ViewBag.RecieptPostOffice as SelectList).First();
            selectedRecieptPostOffice.Selected = true;

            return View();
        }

        [HttpPost]
        public ActionResult CreatePackage(Package package)
        {
            _context.Packages.Add(package);
            _context.SaveChanges();

            return RedirectToAction("PackageTable");
        }

        [Authorize(Roles = "admin")]
        public ActionResult DeletePackage(int id)
        {
            Package? package = _context.Packages.Find(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
                _context.SaveChanges();
            }

            return RedirectToAction("PackageTable");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditPackage(int? id)
        {
            Package? package = _context.Packages.Find(id);
            if (package != null)
            {
                var names = _context.Packages.ToList();
                var status = STATUS_LIST;
                var senders = _context.Users.ToList();
                var recipients = _context.Users.ToList();
                var sendersPostOffice = _context.PostOffices.ToList();
                var recieptsPostOffice = _context.PostOffices.ToList();

                ViewBag.Names = new SelectList(names, "Id", "Name");
                ViewBag.Status = new SelectList(status);
                ViewBag.Senders = new SelectList(senders, "Id", "FIO");
                ViewBag.Recipients = new SelectList(recipients, "Id", "FIO");
                ViewBag.SenderPostOffice = new SelectList(sendersPostOffice, "Id", "Address");
                ViewBag.RecieptPostOffice = new SelectList(recieptsPostOffice, "Id", "Address");

                return View(package);
            }

            RedirectToAction("PackageTable");

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditPackage(Package package)
        {
            var editedPackage = _context.Packages.First(x => x.Id == package.Id);
            editedPackage.Name = package.Name;
            editedPackage.Status = package.Status;
            editedPackage.IdSender = package.IdSender;
            editedPackage.IdRecipient = package.IdRecipient;
            editedPackage.IdSenderPostOffice = package.IdSenderPostOffice;
            editedPackage.IdRecipientPostOffice = package.IdRecipientPostOffice;

            _context.SaveChanges();
            return RedirectToAction("PackageTable");
        }
    }
}
