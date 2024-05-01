using BilirkisiMvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BilirkisiMvc.Controllers
{
    public class HesapController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Giris(GirisModeli girisModeli)
        {
            var user = await userManager.FindByEmailAsync(girisModeli.Eposta);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, girisModeli.Parola, girisModeli.BeniHatirla, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Giris");
        }
    }
}