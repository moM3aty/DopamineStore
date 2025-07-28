using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DopamineStore.ViewModels;
using DopamineStore.Models;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new List<RoleWithUsersViewModel>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                model.Add(new RoleWithUsersViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    UserNames = usersInRole.Select(u => u.FullName).ToList()
                });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    TempData["SuccessMessage"] = "تم إنشاء الصلاحية بنجاح.";
                }
                else
                {
                    TempData["ErrorMessage"] = "هذه الصلاحية موجودة بالفعل.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "اسم الصلاحية مطلوب.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                if (role.Name == "Admin" || role.Name == "Customer")
                {
                    TempData["ErrorMessage"] = "لا يمكن حذف الصلاحيات الأساسية.";
                }
                else
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                    if (usersInRole.Any())
                    {
                        TempData["ErrorMessage"] = "لا يمكن حذف الصلاحية لأنها مرتبطة بمستخدمين.";
                    }
                    else
                    {
                        await _roleManager.DeleteAsync(role);
                        TempData["SuccessMessage"] = "تم حذف الصلاحية بنجاح.";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
