// Services/IDoctorService.cs
using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(int? departmentId = null);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
    }
}