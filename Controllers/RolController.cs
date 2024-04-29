using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BilirkisiMvc.Controllers;

public class RolController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : Controller
{
    public IActionResult Listele()
    {
        return View(roleManager.Roles.ToList());
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(IdentityRole model)
    {
        if (ModelState.IsValid)
        {
            var result = await roleManager.CreateAsync(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Listele");
            }
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    public async Task<IActionResult> Duzenle(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role != null && role.Name != null)
        {
            ViewBag.Users = userManager.GetUsersInRoleAsync(role.Name);
            return View(role);
        }

        return RedirectToAction("Listele");
    }

    [HttpPost]
    public async Task<IActionResult> Duzenle(IdentityRole model)
    {
        if (ModelState.IsValid)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role != null)
            {
                role.Name = model.Name;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Listele");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                if (role.Name != null)
                {   
                ViewBag.Users = userManager.GetUsersInRoleAsync(role.Name);
                }
            }
            else
            {
                ModelState.AddModelError("", "Rol bulunamadı.");
            }
        }
        return View(model);
    }


    public async Task<IActionResult> Sil(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        await roleManager.DeleteAsync(role);
        return RedirectToAction("Listele");
    }

}
