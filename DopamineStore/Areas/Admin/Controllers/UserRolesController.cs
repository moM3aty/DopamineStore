using DopamineStore.Models;
using DopamineStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.FullName,
                Roles = new List<RoleSelection>()
            };

            foreach (var role in _roleManager.Roles)
            {
                model.Roles.Add(new RoleSelection
                {
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "لا يمكن إزالة صلاحيات المستخدم الحالية");
                return View("Index", model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "لا يمكن إضافة الصلاحيات الجديدة للمستخدم");
                return View("Index", model);
            }

            TempData["SuccessMessage"] = "تم تحديث صلاحيات المستخدم بنجاح.";
            return RedirectToAction("Index", "Customers");
        }
    }
}
