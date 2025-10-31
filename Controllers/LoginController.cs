using ClinicalManagementSystem2025.Services;
using ClinicalManagementSystem2025.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClinicalManagementSystem2025.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Authenticate user
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Ensure claims are never null
            var roleName = user.Role?.RoleName ?? "Guest";
            var userName = user.UserName ?? "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            // Store essentials in Session for downstream use (e.g., CreatedBy)
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", roleName);
            HttpContext.Session.SetString("FullName", user.FullName ?? userName);

            // Also store in cookies for compatibility
            Response.Cookies.Append("UserId", user.UserId.ToString());
            Response.Cookies.Append("UserName", userName);
            Response.Cookies.Append("FullName", user.FullName ?? userName);
            Response.Cookies.Append("RoleId", user.RoleId.ToString());

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            // Redirect based on role
            return roleName switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Receptionist" => RedirectToAction("Index", "Receptionist"),
                "Doctor" => RedirectToAction("Index", "Doctor"),
                "LabTechnician" => RedirectToAction("Index", "LabTechnician"),
                "Pharmacist" => RedirectToAction("Index", "Pharmacist"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
