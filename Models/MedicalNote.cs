using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class MedicalNote
    {
        public int MedicalNoteId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        [Display(Name = "Note Title")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Medical Notes")]
        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Diagnosis")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Prescription")]
        public string? Prescription { get; set; }

        [Display(Name = "Follow-up Instructions")]
        public string? FollowUpInstructions { get; set; }

        [Display(Name = "Next Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime? NextAppointmentDate { get; set; }

        [Display(Name = "Note Type")]
        public string NoteType { get; set; } = "Consultation"; // Consultation, Follow-up, Emergency, etc.

        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Normal"; // High, Normal, Low

        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public Appointment? Appointment { get; set; }

        // Display properties
        [Display(Name = "Patient Name")]
        public string PatientName => Patient?.FullName ?? "N/A";

        [Display(Name = "Doctor Name")]
        public string DoctorName => Doctor?.DoctorName ?? "N/A";

        [Display(Name = "Created On")]
        public string CreatedDateFormatted => CreatedDate.ToString("MMM dd, yyyy hh:mm tt");

        // In your MedicalNote.cs model file
      
        
            // Existing properties...

            [StringLength(4000)] // Adjust the length as needed
            public string? LabTests { get; set; }  // Add this line

            // Rest of your properties...
        
    }
}
