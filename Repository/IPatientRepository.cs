using ClinicalManagementSystem2025.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IPatientRepository
    {
        // Core methods
        Task<int> AddPatientAsync(Patient patient);
        Task<int> AddPatientWithAppointmentAsync(Patient patient, int createdByUserId);
        Task<Patient?> GetPatientByIdAsync(int patientId);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<IEnumerable<Patient>> GetPatientsByPhoneAsync(string phone);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<Patient?> SearchPatientByIdAsync(int patientId);

        // Additional methods for service layer
        Task<int> AddAsync(Patient patient);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByPhoneAsync(string phone);
    }
}