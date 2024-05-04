using BilirkisiMvc.Models;
using BilirkisiMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Cikis()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Giris");
        }

        public IActionResult ParolamiUnuttum()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ParolamiUnuttum(string Eposta)
        {
            if (string.IsNullOrEmpty(Eposta))
            {
                TempData["Mesaj"] = "Eposta adresini girin.";
                return View();
            }

            var user = await userManager.FindByEmailAsync(Eposta);

            if (user == null)
            {
                TempData["mesaj"] = "Eposta adresiyle eşleşen bir kullanıcı bulunamadı.";
                return View();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ParolaYenile", "Hesap", new { user.Id, token });
            await epostaGonderici.EpostaGonderAsync(Eposta, "Parola Sıfırlama", $"Parolanızı sıfırlamak için lütfen <a href='http://localhost:5270{url}'>tıklayın</a>.");

            TempData["Mesaj"] = "Parola sıfırlama bağlantısı eposta adresinize gönderildi. Lütfen eposta adresinize erişerek parolanızı sıfırlayın.";
            return View();
        }

        public IActionResult ParolaYenile(string Id, string token)
        {
            if (Id == null || token == null)
            {
                TempData["Mesaj"] = "Geçersiz token bilgisi. Lütfen tekrar deneyiniz.";
                return RedirectToAction("Giris");
            }
            var model = new ParolaYenilemeModeli { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ParolaYenile(ParolaYenilemeModeli model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Eposta);
                if (user == null)
                {
                    TempData[("Mesaj")] = "Bu eposta adresiyle kayıtlı bir kullanıcı bulunamadı.";
                    return RedirectToAction("Giris", "Hesap");
                }
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Parola);
                    if (result.Succeeded)
                    {
                        TempData["Mesaj"] = "Parolanız başarıyla değiştirildi.";
                        return RedirectToAction("Giris", "Hesap");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                TempData["Mesaj"] = "Kullanıcı bulunamadı.";
                return View(model);
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Ekle()
        {
            return View();
        }

        [Authorize]
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

                    await epostaGonderici.EpostaGonderAsync(user.Email, "Hesap Onayı", $"E-posta hesabınızı onaylamak için lütfen <a href='http://localhost:5270{url}'>tıklayın.</a>");

                    TempData["Mesaj"] = "Lütfen eposta adresinize gönderilen bağlantı ile hesabınızı onaylayın.";
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
    
            public IActionResult YetkiHatasi()
        {
            return View();
        }
    }
}