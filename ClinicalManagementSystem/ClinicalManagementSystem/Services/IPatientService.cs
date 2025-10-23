using ClinicalManagementSystem2025.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public interface IPatientService
    {
        // Core patient operations
        Task<int> AddPatientAsync(Patient patient);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetPatientsByPhoneAsync(string phone);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<Patient?> GetPatientByPhoneSingleAsync(string phone);

        // Additional methods for compatibility
        Task<int> AddPatientCompactAsync(Patient patient);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientCompactByIdAsync(int id);

        // NEW: Search patient by ID
        Task<Patient?> SearchPatientByIdAsync(int patientId);
    }
}