using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Appointment Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; } = 30; // Default to 30 minutes

        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled";

        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Patient? Patient { get; set; }

        // Display properties - these are read-only and calculated
        [Display(Name = "Patient Name")]
        public string PatientName => Patient?.FullName ?? "N/A";

        [Display(Name = "Patient Phone")]
        public string PatientPhone => Patient?.Phone ?? "N/A";

        [Display(Name = "Doctor Name")]
        public string DoctorName { get; set; } = string.Empty;

        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Appointment Code")]
        public string AppointmentCode => $"APT{AppointmentId:00000}";

        // Helper property to get the full appointment DateTime
        public DateTime AppointmentDateTime => AppointmentDate.Add(AppointmentTime);
    }
}