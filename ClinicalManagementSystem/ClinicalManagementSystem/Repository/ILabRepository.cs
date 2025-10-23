using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface ILabRepository
    {
        Task<LabTest> GetLabTestByIdAsync(int id);
        Task<IEnumerable<LabTest>> GetAllLabTestsAsync();
        Task<LabTest> AddLabTestAsync(LabTest labTest);
        Task<LabTest> UpdateLabTestAsync(LabTest labTest);
        Task<bool> DeleteLabTestAsync(int id);

        Task<LabReport> GetLabReportByIdAsync(int id);
        Task<IEnumerable<LabReport>> GetAllLabReportsAsync();
        Task<IEnumerable<LabReport>> GetLabReportsByPatientAsync(int patientId);
        Task<IEnumerable<LabReport>> GetLabReportsByDoctorAsync(int doctorId);
        Task<IEnumerable<LabReport>> GetPendingLabReportsAsync();
        Task<LabReport> AddLabReportAsync(LabReport labReport);
        Task<LabReport> UpdateLabReportAsync(LabReport labReport);
        Task<bool> DeleteLabReportAsync(int id);
    }
}