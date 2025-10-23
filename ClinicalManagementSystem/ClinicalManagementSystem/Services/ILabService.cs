using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Services
{
    public interface ILabService
    {
        Task<LabTest> GetLabTestByIdAsync(int id);
        Task<IEnumerable<LabTest>> GetAllLabTestsAsync();
        Task<LabTest> CreateLabTestAsync(LabTest labTest);
        Task<LabTest> UpdateLabTestAsync(int id, LabTest labTest);
        Task<bool> DeleteLabTestAsync(int id);

        Task<LabReport> GetLabReportByIdAsync(int id);
        Task<IEnumerable<LabReport>> GetAllLabReportsAsync();
        Task<IEnumerable<LabReport>> GetLabReportsByPatientAsync(int patientId);
        Task<IEnumerable<LabReport>> GetPendingLabReportsAsync();
        Task<LabReport> CreateLabReportAsync(LabReport labReport);
        Task<LabReport> UpdateLabReportAsync(int id, LabReport labReport);
        Task<bool> DeleteLabReportAsync(int id);
    }
}