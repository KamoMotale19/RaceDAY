using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services; 
using System.Security.Claims;

namespace RaceDayWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginResult = await _apiService.LoginAsync(email, password);

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetString("JwtToken", loginResult.Token);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginResult.Email),
                new Claim(ClaimTypes.Role, loginResult.Role),
                new Claim("FirstName", loginResult.FirstName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (loginResult.Role == "Organiser")
            {
                return RedirectToAction("Dashboard", "Organiser");
            }
            else
            {
                return RedirectToAction("BrowseEvents", "Participant"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}