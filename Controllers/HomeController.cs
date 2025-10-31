using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClinicalManagementSystem2025.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                else if (User.IsInRole("Doctor"))
                    return RedirectToAction("Index", "Doctor");
                else if (User.IsInRole("Receptionist"))
                    return RedirectToAction("Index", "Receptionist");
                else if (User.IsInRole("LabTechnician"))
                    return RedirectToAction("Index", "LabTechnician");
                else if (User.IsInRole("Pharmacist"))
                    return RedirectToAction("Index", "Pharmacist");
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TestRazor()
        {
            ViewBag.TestMessage = "Razor views are working correctly!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return View();
        }
    }
}
