using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.Controllers;

public class AccountController(DataService data, IHostApplicationLifetime appLifetime) : Controller
{
    private const int MaxAttempts = 3;

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        var attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;
        return View(new LoginViewModel { FailedAttempts = attempts });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        int attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

        if (attempts >= MaxAttempts)
        {
            model.ErrorMessage = "Túl sok sikertelen próbálkozás. A program leáll.";
            model.FailedAttempts = attempts;
            appLifetime.StopApplication();
            return View("Lockout");
        }

        var librarian = data.Authenticate(model.Username, model.Password);
        if (librarian == null)
        {
            attempts++;
            HttpContext.Session.SetInt32("LoginAttempts", attempts);

            if (attempts >= MaxAttempts)
            {
                appLifetime.StopApplication();
                return View("Lockout");
            }

            model.ErrorMessage = $"Hibás felhasználónév vagy jelszó. ({attempts}/{MaxAttempts} próbálkozás)";
            model.FailedAttempts = attempts;
            return View(model);
        }

        HttpContext.Session.Remove("LoginAttempts");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, librarian.Username),
            new(ClaimTypes.GivenName, librarian.FullName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = null // Session cookie
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProperties);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
