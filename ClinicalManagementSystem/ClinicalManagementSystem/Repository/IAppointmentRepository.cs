using ClinicalManagementSystem2025.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IAppointmentRepository
    {
        Task<int> AddAppointmentAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetDoctorAppointmentsForDateAsync(int doctorId, DateTime date);
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync();
        Task<bool> CreateQuickAppointmentForNewPatientAsync(int patientId, int createdByUserId);
    }
}