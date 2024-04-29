using BilirkisiMvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilirkisiMvc.Controllers;

public class KullaniciController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    public IActionResult Listele()
    {
        return View(userManager.Users);
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(KullaniciOlusturmaModeli model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            IdentityResult result = await userManager.CreateAsync(user, model.Password);
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
        if (id == null)
        {
            return RedirectToAction("Listele");
        }
        var user = await userManager.FindByIdAsync(id);
        if (user != null)
        {
            ViewBag.Roles = await roleManager.Roles.Select(i => i.Name).ToListAsync();
            return View(new KullaniciDuzenlemeModeli
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                SeciliRol = await userManager.GetRolesAsync(user)
            });
        }
        return RedirectToAction("Listele");
    }

    [HttpPost]
    public async Task<IActionResult> Duzenle(string id, KullaniciDuzenlemeModeli model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Listele");
        }
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                {
                    //var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    await userManager.RemovePasswordAsync(user);
                    await userManager.AddPasswordAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
                    if (model.SeciliRol != null)
                    {
                        await userManager.AddToRolesAsync(user, model.SeciliRol);
                    }
                    return RedirectToAction("Listele");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sil(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Listele");
            }
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return RedirectToAction("Listele");
    }
}