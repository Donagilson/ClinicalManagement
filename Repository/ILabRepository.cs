using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface ILabRepository
    {
        Task<LabTest> AddLabTestAsync(LabTest labTest);
        Task<LabTest> UpdateLabTestAsync(LabTest labTest);
        Task<bool> DeleteLabTestAsync(int id);

        Task<LabTest?> GetLabTestByIdAsync(int id);
        Task<IEnumerable<LabTest>> GetAllLabTestsAsync();

        Task<LabReport?> GetLabReportByIdAsync(int id);
        Task<IEnumerable<LabReport>> GetAllLabReportsAsync();
        Task<IEnumerable<LabReport>> GetLabReportsByPatientAsync(int patientId);
        Task<IEnumerable<LabReport>> GetLabReportsByDoctorAsync(int doctorId);
        Task<IEnumerable<LabReport>> GetLabReportsByTechnicianAsync(int technicianId);
        Task<IEnumerable<LabReport>> GetPendingLabReportsAsync();
        Task<LabReport> AddLabReportAsync(LabReport labReport);
        Task<LabReport> UpdateLabReportAsync(LabReport labReport);
        Task<bool> DeleteLabReportAsync(int id);

        // Lab Test Prescription methods
        Task<LabTestPrescription> AddLabTestPrescriptionAsync(LabTestPrescription prescription);
        Task<LabTestPrescription?> GetLabTestPrescriptionByIdAsync(int id);
        Task<IEnumerable<LabTestPrescription>> GetAllLabTestPrescriptionsAsync();
        Task<IEnumerable<LabTestPrescription>> GetLabTestPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<LabTestPrescription>> GetLabTestPrescriptionsByDoctorAsync(int doctorId);
        Task<IEnumerable<LabTestPrescription>> GetPendingLabTestPrescriptionsAsync();
        Task<IEnumerable<LabTestPrescription>> GetAssignedLabTestPrescriptionsAsync(int technicianId);
        Task<LabTestPrescription> UpdateLabTestPrescriptionAsync(LabTestPrescription prescription);
        Task<bool> DeleteLabTestPrescriptionAsync(int id);
        Task<bool> AssignTechnicianToPrescriptionAsync(int prescriptionId, int technicianId);
    }
}