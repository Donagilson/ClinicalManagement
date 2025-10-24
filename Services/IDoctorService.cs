using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Services
{
    public interface IDoctorService
    {
        Task<List<Appointment>> GetTodayAppointments(int doctorId);
        Task<Patient> GetPatientDetails(int patientId);
        Task<List<Prescription>> GetPatientPrescriptions(int patientId);
        Task<List<LabReport>> GetPatientLabReports(int patientId);
        Task<List<Medicine>> GetActiveMedicines();
        Task<bool> SaveConsultation(int appointmentId, string symptoms, string diagnosis, string notes, int doctorId);
        Task<bool> PrescribeMedicines(Prescription prescription, List<PrescriptionDetail> prescriptionDetails);
        Task<bool> RequestLabTest(LabReport labReport);
        Task<int> GetDoctorIdByUserId(int userId);
        Task<List<Patient>> GetDoctorPatients(int doctorId);
        Task<bool> SaveConsultationNotes(ConsultationNotes consultationNotes);
        Task<List<ConsultationNotes>> GetPatientConsultationNotes(int patientId);
        Task<int> SendPrescriptionToPharmacy(int prescriptionId, int patientId, int doctorId, string notes);
        Task<List<PharmacyPrescriptionViewModel>> GetPharmacyPrescriptions(string status = null);
        Task<bool> UpdatePharmacyPrescriptionStatus(int pharmacyPrescriptionId, string status, int pharmacistId);
    }
}