using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public Task<int> AddAppointmentAsync(Appointment appointment) => _repo.AddAppointmentAsync(appointment);

        public Task<IEnumerable<Appointment>> GetAllAsync() => _repo.GetAllAsync();

        public Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId) => _repo.GetAppointmentsByPatientIdAsync(patientId);

        public Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId) => _repo.GetByDoctorIdAsync(doctorId);

        public Task<IEnumerable<Appointment>> GetDoctorAppointmentsForDateAsync(int doctorId, DateTime date) => _repo.GetDoctorAppointmentsForDateAsync(doctorId, date);

        public Task<Appointment?> GetAppointmentByIdAsync(int appointmentId) => _repo.GetAppointmentByIdAsync(appointmentId);

        public Task<bool> UpdateAppointmentAsync(Appointment appointment) => _repo.UpdateAppointmentAsync(appointment);

        public Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status) => _repo.UpdateAppointmentStatusAsync(appointmentId, status);

        public Task<bool> DeleteAppointmentAsync(int appointmentId) => _repo.DeleteAppointmentAsync(appointmentId);

        public Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync() => _repo.GetTodaysAppointmentsAsync();

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDateTime, int durationMinutes = 30)
        {
            // Get all appointments for the doctor on the specified date
            var appointments = await _repo.GetDoctorAppointmentsForDateAsync(doctorId, appointmentDateTime.Date);

            // Check if any existing appointment overlaps with the proposed time slot
            var proposedStart = appointmentDateTime;
            var proposedEnd = appointmentDateTime.AddMinutes(durationMinutes);

            foreach (var appointment in appointments)
            {
                var existingStart = appointment.AppointmentDate.Add(appointment.AppointmentTime);
                var existingEnd = existingStart.AddMinutes(appointment.DurationMinutes);

                // Check for overlap
                if ((proposedStart >= existingStart && proposedStart < existingEnd) ||
                    (proposedEnd > existingStart && proposedEnd <= existingEnd) ||
                    (proposedStart <= existingStart && proposedEnd >= existingEnd))
                {
                    return false; // Overlap found, doctor is not available
                }
            }

            return true; // No overlap, doctor is available
        }
    }
}