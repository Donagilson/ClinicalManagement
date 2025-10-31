using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;

        public PatientService(IPatientRepository repo)
        {
            _repo = repo ?? throw new System.ArgumentNullException(nameof(repo));
        }

        // Core methods
        public Task<int> AddPatientAsync(Patient patient) => _repo.AddPatientAsync(patient);
        public Task<Patient?> GetPatientByIdAsync(int id) => _repo.GetPatientByIdAsync(id);
        public Task<IEnumerable<Patient>> GetPatientsByPhoneAsync(string phone) => _repo.GetPatientsByPhoneAsync(phone);
        public Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm) => _repo.SearchPatientsAsync(searchTerm);

        public async Task<Patient?> GetPatientByPhoneSingleAsync(string phone)
        {
            var patients = await _repo.GetPatientsByPhoneAsync(phone);
            return patients.FirstOrDefault();
        }

        // Additional methods
        public Task<int> AddPatientCompactAsync(Patient patient) => _repo.AddAsync(patient);

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public Task<Patient?> SearchPatientByIdAsync(int patientId)
        {
            return _repo.SearchPatientByIdAsync(patientId);
        }

        public Task<Patient?> GetPatientCompactByIdAsync(int id) => _repo.GetByIdAsync(id);
    }
}