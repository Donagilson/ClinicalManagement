using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(int? departmentId = null);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Doctor?> GetDoctorByIdAsync(int doctorId);
        Task<List<DoctorDto>?> GetDoctorsByDepartmentAsync(int departmentId);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();

        // New methods for doctor management
        Task<int> AddDoctorAsync(Doctor doctor);
        Task<bool> UpdateDoctorAsync(Doctor doctor);
        Task<bool> DeleteDoctorAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllDoctorsWithDetailsAsync();

        // Specialization methods
        Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
        Task<IEnumerable<Specialization>> GetSpecializationsByDepartmentAsync(int departmentId);
    }
}
