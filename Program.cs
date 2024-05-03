using BilirkisiMvc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syncfusion.MVC;
using Syncfusion.Licensing;
using BilirkisiMvc.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEpostaGonderici, SmtpEpostaGonderici>(i => 
    new SmtpEpostaGonderici(
        builder.Configuration["EpostaGonderici:Sunucu"], 
        builder.Configuration.GetValue<int>("EpostaGonderici:Port"), 
        builder.Configuration.GetValue<bool>("EpostaGonderici:SslAktif"), 
        builder.Configuration["EpostaGonderici:KullaniciAdi"], 
        builder.Configuration["EpostaGonderici:Parola"])
        );

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VtBaglaci>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<VtBaglaci>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{

    options.Lockout.AllowedForNewUsers = true; // Yeni kullanıcılar için varsayılan olarak kilitlenme özelliği açık olacak.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});


Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzIzMDA2OEAzMjM1MmUzMDJlMzBNVm95S1dkOTdncEgwb0trb2FCRXltWmpnbUJBWDVzQjdPaUpaZmlRaE5FPQ==");

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Hesap/Giris";
    options.AccessDeniedPath = "/Hesap/YetkiHatasi";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

IdentitySeedData.IdentityTestUser(app);

app.Run();
