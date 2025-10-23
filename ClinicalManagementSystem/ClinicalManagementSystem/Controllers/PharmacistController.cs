// Controllers/PharmacistController.cs
using Microsoft.AspNetCore.Mvc;

namespace ClinicalManagementSystem2025.Controllers
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Pharmacist Dashboard";
            ViewBag.Role = "Pharmacist";
            ViewBag.UserName = Request.Cookies["FullName"];
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("FullName");
            Response.Cookies.Delete("RoleId");

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Login");
        }
    }
}
