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
        public DateTime AppointmentDate { get; set; } = DateTime.Today;

        [Display(Name = "Appointment Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; } = TimeSpan.Zero;

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; } = 30;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, InProgress, Visited, Completed, Cancelled

        // Helper property to check if patient has been visited
        public bool IsVisited => Status == "Visited" || Status == "Completed";

        // Helper property to check if appointment is active (not completed or cancelled)
        public bool IsActive => Status != "Completed" && Status != "Cancelled";

        // Helper property to check if appointment needs attention
        public bool NeedsAttention => Status == "Scheduled" || Status == "Confirmed";

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
        public string? DoctorName { get; set; }

        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        [Display(Name = "Appointment Code")]
        public string AppointmentCode => $"APT{AppointmentId:00000}";

        // Helper property to get the full appointment DateTime
        public DateTime AppointmentDateTime => AppointmentDate.Add(AppointmentTime);
    }
}