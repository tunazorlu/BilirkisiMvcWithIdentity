using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BilirkisiMvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace BilirkisiMvc.Controllers;

[Authorize]
#pragma warning disable CS9113 // Parameter is unread.
public class HomeController(ILogger<HomeController> logger) : Controller
#pragma warning restore CS9113 // Parameter is unread.
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
