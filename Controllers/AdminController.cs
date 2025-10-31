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
            // Password is not required when editing user profile
            ModelState.Remove("UserPassword");

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
            var departments = await _doctorRepository.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(Doctor doctor, string Email, string Phone, string Username, string Password)
        {
            // Remove UserId validation since we're creating the user automatically
            ModelState.Remove("UserId");
            
            // Remove Phone and Email validation since they're optional
            ModelState.Remove("Phone");
            ModelState.Remove("Email");
            
            // Remove SpecializationId validation since we're using Specialization text field
            ModelState.Remove("SpecializationId");
            
            // Validate required fields manually
            if (string.IsNullOrWhiteSpace(doctor.DoctorName))
            {
                ModelState.AddModelError("DoctorName", "Doctor name is required.");
            }
            
            if (string.IsNullOrWhiteSpace(Username))
            {
                ModelState.AddModelError("Username", "Username is required.");
            }
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }
            else if (Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
            }
            
            if (doctor.DepartmentId <= 0)
            {
                ModelState.AddModelError("DepartmentId", "Please select a department.");
            }

            // Check if username already exists
            if (!string.IsNullOrWhiteSpace(Username))
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                if (allUsers.Any(u => u.UserName.Equals(Username, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Username", "Username already exists. Please choose a different username.");
                }
            }
            
            if (!ModelState.IsValid)
            {
                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                ViewBag.Departments = departments;
                return View(doctor);
            }

            try
            {
                // Create a user account for the doctor first using provided credentials
                var newUser = new User
                {
                    UserName = Username,
                    FullName = doctor.DoctorName,
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email,
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                    UserPassword = Password, // Use provided password
                    RoleId = await GetDoctorRoleIdAsync(), // Get Doctor role ID
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // Add the user first
                await _userRepository.AddUserAsync(newUser);
                
                // Get the created user to get the UserId by finding it in all users
                var allUsers = await _userRepository.GetAllUsersAsync();
                var createdUser = allUsers.FirstOrDefault(u => u.UserName == newUser.UserName);
                if (createdUser != null)
                {
                    doctor.UserId = createdUser.UserId;
                }
                else
                {
                    throw new Exception("Failed to create user account for doctor");
                }

                // Set SpecializationId to null since we're using text-based specialization
                doctor.SpecializationId = null;

                // Now add the doctor with the UserId
                await _doctorRepository.AddDoctorAsync(doctor);
                
                TempData["SuccessMessage"] = $"Doctor added successfully! Username: {Username}, Password: {Password}";
                return RedirectToAction("ManageDoctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding doctor: {ex.Message}");

                var departments = await _doctorRepository.GetAllDepartmentsAsync();
                ViewBag.Departments = departments;

                return View(doctor);
            }
        }

        private async Task<int> GetDoctorRoleIdAsync()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            var doctorRole = roles.FirstOrDefault(r => r.RoleName.Equals("Doctor", StringComparison.OrdinalIgnoreCase));
            return doctorRole?.RoleId ?? 2; // Default to 2 if Doctor role not found
        }

        private async Task<string> GenerateUniqueUsernameAsync(string baseUsername)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            var existingUsernames = allUsers.Select(u => u.UserName.ToLower()).ToHashSet();

            string candidateUsername = baseUsername;
            int counter = 1;

            // Keep trying until we find a unique username
            while (existingUsernames.Contains(candidateUsername))
            {
                candidateUsername = $"{baseUsername}{counter}";
                counter++;
            }

            return candidateUsername;
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
