using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Appointment code is required")]
        [StringLength(20, ErrorMessage = "Appointment code cannot exceed 20 characters")]
        [Display(Name = "Appointment Code")]
        public string AppointmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Patient is required")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date and time is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date & Time")]
        public DateTime AppointmentDate { get; set; }

        [Range(1, 480, ErrorMessage = "Duration must be between 1 and 480 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; } = 30;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled";

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        [Display(Name = "Reason for Visit")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        // Display properties
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // Validation method
        public bool IsPastAppointment => AppointmentDate < DateTime.Now;
    }
}