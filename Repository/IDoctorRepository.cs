using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IDoctorRepository
    {
        // Get today's appointments for doctor
        Task<List<Appointment>> GetTodayAppointments(int doctorId);

        // Get patient details
        Task<Patient> GetPatientById(int patientId);

        // Get patient medical history
        Task<List<Prescription>> GetPatientPrescriptions(int patientId);
        Task<List<LabReport>> GetPatientLabReports(int patientId);
        Task<List<ConsultationNotes>> GetPatientConsultationNotes(int patientId);

        // Get available medicines
        Task<List<Medicine>> GetActiveMedicines();

        // Save consultation
        Task<bool> UpdateAppointment(int appointmentId, string symptoms, string diagnosis, string notes);
        Task<bool> SaveConsultationNotes(ConsultationNotes consultationNotes);

        // Prescribe medicines
        Task<int> CreatePrescription(Prescription prescription);
        Task<bool> AddPrescriptionDetails(List<PrescriptionDetail> prescriptionDetails);

        // Request lab tests
        Task<bool> CreateLabReport(LabReport labReport);

        // Get doctor by user ID
        Task<int> GetDoctorIdByUserId(int userId);

       

        // Get all patients for a specific doctor
        Task<List<Patient>> GetDoctorPatients(int doctorId);

        // Get all appointments for a specific doctor (not just today)
        Task<List<Appointment>> GetDoctorAppointments(int doctorId);

        // Get appointments by date range for a doctor
        Task<List<Appointment>> GetDoctorAppointmentsByDate(int doctorId, DateTime startDate, DateTime endDate);

        // Get all prescriptions for a specific doctor
        Task<List<Prescription>> GetDoctorPrescriptions(int doctorId);

        // Get prescription details with medicine information
        Task<List<PrescriptionDetail>> GetPrescriptionDetails(int prescriptionId);

        // Get all lab reports for a specific doctor
        Task<List<LabReport>> GetDoctorLabReports(int doctorId);

        // Get consultation notes for a doctor
        Task<List<ConsultationNotes>> GetDoctorConsultationNotes(int doctorId);

        // Get patient full medical history
        Task<PatientMedicalHistory> GetPatientMedicalHistory(int patientId);

        // Update appointment status
        Task<bool> UpdateAppointmentStatus(int appointmentId, string status);

        // Get doctor dashboard statistics
        Task<DoctorStatistics> GetDoctorStatistics(int doctorId);

        // Add these methods to your existing interface
        Task<int> SendPrescriptionToPharmacy(int prescriptionId, int patientId, int doctorId, string notes);
        Task<List<PharmacyPrescriptionViewModel>> GetPharmacyPrescriptions(string status = null);
        Task<bool> UpdatePharmacyPrescriptionStatus(int pharmacyPrescriptionId, string status, int pharmacistId);
    }

    // Additional model classes for statistics and medical history
    public class PatientMedicalHistory
    {
        public Patient Patient { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<LabReport> LabReports { get; set; }
        public List<ConsultationNotes> ConsultationNotes { get; set; }
    }

    public class DoctorStatistics
    {
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalPrescriptions { get; set; }
        public int PendingLabReports { get; set; }
        public int CompletedAppointments { get; set; }
    }
}