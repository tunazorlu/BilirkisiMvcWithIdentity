using BilirkisiMvc.Models;
using BilirkisiMvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BilirkisiMvc.Controllers
{
    public class HesapController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IEpostaGonderici epostaGonderici) : Controller
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

                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Lütfen eposta adresinize gönderilen bağlantı ile hesabınızı onaylayınız.");
                        return View("Giris");
                    }

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
                        ModelState.AddModelError("", $"Hesabınız kilitlendi. {timeLeft.Minutes + 1} dakika sonra tekrar deneyebilirsiniz.");
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
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("EpostaOnayla", "Hesap", new { user.Id, token });

                    await epostaGonderici.EpostaGonderAsync(user.Email, "Hesap Onayı", $"E-posta hesabınızı onaylamak için lütfen <a href='http://localhost:5270{url}'>tıklayınız</a>.");
                    
                    TempData["Mesaj"] = "Lütfen eposta adresinize gönderilen bağlantı ile hesabınızı onaylayınız.";
                    return RedirectToAction("Giris", "Hesap");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EpostaOnayla(string Id, string token)
        {
            if (Id == null || token == null)
            {
                TempData["Mesaj"] = "Geçersiz token bilgisi. Lütfen tekrar deneyiniz.";
                return View();
            }
            var user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData["Mesaj"] = "Hesabınız onaylandı.";
                    return RedirectToAction("Giris", "Hesap");
                }
            }
            TempData["Mesaj"] = "Kullanıcı bulunamadı.";
            return View("");
        }
    }
}