using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> AddPrescriptionAsync(Prescription prescription);
        Task<Prescription?> GetPrescriptionByIdAsync(int id);
        Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync();
        Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<bool> UpdatePrescriptionAsync(Prescription prescription);
        Task<bool> DeletePrescriptionAsync(int id);

        Task<PrescriptionDetail> AddPrescriptionDetailAsync(PrescriptionDetail prescriptionDetail);
        Task<IEnumerable<PrescriptionDetail>> GetPrescriptionDetailsAsync(int prescriptionId);
        Task<bool> UpdatePrescriptionDetailAsync(PrescriptionDetail prescriptionDetail);
        Task<bool> DeletePrescriptionDetailAsync(int id);
    }
}
