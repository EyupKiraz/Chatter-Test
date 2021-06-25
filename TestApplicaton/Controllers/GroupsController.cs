using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApplicaton.Data;
using TestApplicaton.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace TestApplicaton.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public GroupsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                var groups = _dbContext.Groups.ToList();

                ViewBag.Groups = groups;
                ViewBag.User = user;
                ViewBag.UserGroup = user.GroupId != 0;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JoinGroup(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            user.GroupId = id;

            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup()
        {
            var user = await _userManager.GetUserAsync(User);
            user.GroupId = 0;

            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Groups");
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(string name, string description)
        {
            var group = new Group();
            group.Name = name;
            group.Description = description;

            _dbContext.Add(group);
            _dbContext.SaveChanges();

            var user = await _userManager.GetUserAsync(User);
            user.GroupId = group.Id;

            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Groups");
        }
    }
}