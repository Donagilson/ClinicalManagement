using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;

namespace ClinicalManagementSystem2025.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<List<Appointment>> GetTodayAppointments(int doctorId)
        {
            return await _doctorRepository.GetTodayAppointments(doctorId);
        }

        public async Task<Patient> GetPatientDetails(int patientId)
        {
            return await _doctorRepository.GetPatientById(patientId);
        }

        public async Task<List<Prescription>> GetPatientPrescriptions(int patientId)
        {
            return await _doctorRepository.GetPatientPrescriptions(patientId);
        }

        public async Task<List<LabReport>> GetPatientLabReports(int patientId)
        {
            return await _doctorRepository.GetPatientLabReports(patientId);
        }

        public async Task<List<Medicine>> GetActiveMedicines()
        {
            return await _doctorRepository.GetActiveMedicines();
        }

        public async Task<bool> SaveConsultation(int appointmentId, string symptoms, string diagnosis, string notes, int doctorId)
        {
            return await _doctorRepository.UpdateAppointment(appointmentId, symptoms, diagnosis, notes);
        }

        public async Task<bool> PrescribeMedicines(Prescription prescription, List<PrescriptionDetail> prescriptionDetails)
        {
            try
            {
                var prescriptionId = await _doctorRepository.CreatePrescription(prescription);
                if (prescriptionId > 0)
                {
                    foreach (var detail in prescriptionDetails)
                    {
                        detail.PrescriptionId = prescriptionId;
                    }
                    return await _doctorRepository.AddPrescriptionDetails(prescriptionDetails);
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in PrescribeMedicines: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestLabTest(LabReport labReport)
        {
            return await _doctorRepository.CreateLabReport(labReport);
        }

        public async Task<int> GetDoctorIdByUserId(int userId)
        {
            return await _doctorRepository.GetDoctorIdByUserId(userId);
        }
        
        public async Task<List<Patient>> GetDoctorPatients(int doctorId)
        {
            return await _doctorRepository.GetDoctorPatients(doctorId);
        }

        public async Task<bool> SaveConsultationNotes(ConsultationNotes consultationNotes)
        {
            return await _doctorRepository.SaveConsultationNotes(consultationNotes);
        }

        public async Task<List<ConsultationNotes>> GetPatientConsultationNotes(int patientId)
        {
            return await _doctorRepository.GetPatientConsultationNotes(patientId);
        }
        public async Task<int> SendPrescriptionToPharmacy(int prescriptionId, int patientId, int doctorId, string notes)
        {
            return await _doctorRepository.SendPrescriptionToPharmacy(prescriptionId, patientId, doctorId, notes);
        }

        public async Task<List<PharmacyPrescriptionViewModel>> GetPharmacyPrescriptions(string status = null)
        {
            return await _doctorRepository.GetPharmacyPrescriptions(status);
        }

        public async Task<bool> UpdatePharmacyPrescriptionStatus(int pharmacyPrescriptionId, string status, int pharmacistId)
        {
            return await _doctorRepository.UpdatePharmacyPrescriptionStatus(pharmacyPrescriptionId, status, pharmacistId);
        }
       
    }
}