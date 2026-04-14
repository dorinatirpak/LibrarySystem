using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class HomeController(DataService data) : Controller
{
    public IActionResult Index() => View(data.GetDashboard());
}
