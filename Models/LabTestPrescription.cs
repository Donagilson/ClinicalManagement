using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class LabTestPrescription
    {
        [Key]
        public int LabTestPrescriptionId { get; set; }

        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int LabTestId { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? ClinicalNotes { get; set; }

        [StringLength(100)]
        public string? Priority { get; set; } = "Normal"; // Normal, Urgent, Routine

        [StringLength(50)]
        public string? Status { get; set; } = "Prescribed"; // Prescribed, Collected, InProgress, Completed, Cancelled

        public DateTime? SampleCollectionDate { get; set; }

        public int? AssignedTechnicianId { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Appointment? Appointment { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public virtual LabTest? LabTest { get; set; }

        // Display properties
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? TestName { get; set; }
        public string? TestCode { get; set; }
        public decimal TestPrice { get; set; }
        public string? LabTechnicianName { get; set; }

        // Helper properties
        public string PrescriptionCode => $"LTP{AppointmentId:00000}-{LabTestPrescriptionId:000}";
        public bool IsUrgent => Priority == "Urgent";
        public bool IsCompleted => Status == "Completed";
        public bool IsInProgress => Status == "InProgress" || Status == "Collected";
    }
}
