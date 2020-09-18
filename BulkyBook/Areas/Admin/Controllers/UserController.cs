using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(string id)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ApplicationUser applicationUser)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(s => s.Id == applicationUser.Id);
            var oldrole = _db.UserRoles.FirstOrDefault(s => s.UserId == user.Id);
            var oldrolename = _db.Roles.FirstOrDefault(s => s.Id == oldrole.RoleId);
            var newrole = _db.Roles.FirstOrDefault(s => s.Name == applicationUser.Role);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(oldrole.RoleId))
                {
                    _userManager.RemoveFromRoleAsync(user, oldrolename.Name).Wait();
                }
                _userManager.AddToRoleAsync(user, newrole.Name).Wait();

            }
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }

      

        #endregion
    }
}