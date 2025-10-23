using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using ClinicalManagementSystem2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;

        public AdminController(IUserService userService, IUserRepository userRepository, IDoctorRepository doctorRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _userRepository.GetAllRolesAsync();
                ViewBag.Roles = roles;
                return View(user);
            }

            await _userRepository.AddUserAsync(user);
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Index");
            }

            var roles = await _userRepository.GetAllRolesAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _userRepository.GetAllRolesAsync();
                ViewBag.Roles = roles;
                return View(user);
            }

            await _userRepository.UpdateUserAsync(user);
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Index");
            }

            await _userRepository.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                TempData["ErrorMessage"] = "Password cannot be empty!";
                return RedirectToAction("Index");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Index");
            }

            await _userRepository.ResetPasswordAsync(id, newPassword);
            TempData["SuccessMessage"] = "Password reset successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Index");
            }

            user.IsActive = !user.IsActive;
            await _userRepository.UpdateUserAsync(user);

            var status = user.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"User {status} successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddDoctor()
        {
            var users = await _userRepository.GetUsersByRoleAsync("Doctor");
            var departments = await _doctorRepository.GetAllDepartmentsAsync();
            var specializations = await _doctorRepository.GetAllSpecializationsAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;
            ViewBag.Specializations = specializations;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userRepository.GetUsersByRoleAsync("Doctor");
                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                var specializations = await _doctorRepository.GetAllSpecializationsAsync();

                ViewBag.Users = users;
                ViewBag.Departments = departments;
                ViewBag.Specializations = specializations;

                return View(doctor);
            }

            try
            {
                await _doctorRepository.AddDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Doctor added successfully!";
                return RedirectToAction("ManageDoctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding doctor: {ex.Message}");

                var users = await _userRepository.GetUsersByRoleAsync("Doctor");
                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                var specializations = await _doctorRepository.GetAllSpecializationsAsync();

                ViewBag.Users = users;
                ViewBag.Departments = departments;
                ViewBag.Specializations = specializations;

                return View(doctor);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageDoctors()
        {
            var doctors = await _doctorRepository.GetAllDoctorsWithDetailsAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found!";
                return RedirectToAction("ManageDoctors");
            }

            var users = await _userRepository.GetUsersByRoleAsync("Doctor");
            var departments = await _doctorRepository.GetAllDepartmentsAsync();
            var specializations = await _doctorRepository.GetAllSpecializationsAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;
            ViewBag.Specializations = specializations;

            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userRepository.GetUsersByRoleAsync("Doctor");
                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                var specializations = await _doctorRepository.GetAllSpecializationsAsync();

                ViewBag.Users = users;
                ViewBag.Departments = departments;
                ViewBag.Specializations = specializations;

                return View(doctor);
            }

            try
            {
                await _doctorRepository.UpdateDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Doctor updated successfully!";
                return RedirectToAction("ManageDoctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating doctor: {ex.Message}");

                var users = await _userRepository.GetUsersByRoleAsync("Doctor");
                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                var specializations = await _doctorRepository.GetAllSpecializationsAsync();

                ViewBag.Users = users;
                ViewBag.Departments = departments;
                ViewBag.Specializations = specializations;

                return View(doctor);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                await _doctorRepository.DeleteDoctorAsync(id);
                TempData["SuccessMessage"] = "Doctor deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting doctor: {ex.Message}";
            }

            return RedirectToAction("ManageDoctors");
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecializationsByDepartment(int departmentId)
        {
            try
            {
                var specializations = await _doctorRepository.GetSpecializationsByDepartmentAsync(departmentId);
                return Json(new { success = true, specializations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
