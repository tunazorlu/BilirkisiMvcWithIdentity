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
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(girisModeli.Eposta);

                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(user, girisModeli.Parola, girisModeli.BeniHatirla, true);

                    if (result.Succeeded)
                    {
                        //Login işlemi başarılı iken kullanıcı girişi için Lockout süresi ve deneme sayısını sıfırlıyoruz.
                        await userManager.ResetAccessFailedCountAsync(user);
                        await userManager.SetLockoutEndDateAsync(user, null);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutDate = await userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value.Subtract(DateTime.UtcNow);
                        ModelState.AddModelError("", $"Hesabınız kilitlendi. {timeLeft.Minutes} dakika sonra tekrar deneyebilirsiniz.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Parolanız hatalı.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu eposta ile kayıtlı bir kullanıcı bulunamadı.");
                }
            }
            return View("Giris");
        }
    }
}