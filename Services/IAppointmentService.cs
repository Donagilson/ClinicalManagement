using ClinicalManagementSystem2025.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public interface IAppointmentService
    {
        Task<int> AddAppointmentAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetDoctorAppointmentsForDateAsync(int doctorId, DateTime date);
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDateTime, int durationMinutes = 30);
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync();
    }
}