using DopamineStore.ViewModels;
using DopamineStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CustomersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string searchString, string roleName)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FullName.Contains(searchString) || u.Email.Contains(searchString));
            }

            var userList = await users.ToListAsync();
            var userRolesViewModel = new List<CustomerViewModel>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new CustomerViewModel
                {
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            if (!string.IsNullOrEmpty(roleName))
            {
                userRolesViewModel = userRolesViewModel.Where(u => u.Roles.Contains(roleName)).ToList();
            }

            ViewData["Roles"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", roleName);
            ViewData["CurrentFilter"] = searchString;

            return View(userRolesViewModel);
        }
    }
}
