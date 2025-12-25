using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // ================== INDEX ADMIN ==================
        public IActionResult Index()
        {
            return View();
        }

        // ================== LISTE DES ROLES ==================
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // ================== CREER UN ROLE ==================
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.RoleName };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction(nameof(ListRoles));

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // ================== EDITER UN ROLE ==================
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return View("NotFound");

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user.UserName);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
                return View("NotFound");

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return RedirectToAction(nameof(ListRoles));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ================== MODIFIER LES UTILISATEURS DANS UN ROLE ==================
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return View("NotFound");

            ViewBag.RoleId = roleId;

            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return View("NotFound");

            foreach (var item in model)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                if (user == null) continue;

                if (item.IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                    await _userManager.AddToRoleAsync(user, role.Name);
                else if (!item.IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
            }

            return RedirectToAction("EditRole", new { id = roleId });
        }

        // ================== SUPPRIMER UN ROLE ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return View("NotFound");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction(nameof(ListRoles));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction(nameof(ListRoles));
        }
    }
}
