using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class LabReport
    {
        [Key]
        public int LabReportId { get; set; }

        [Required]
        [StringLength(20)]
        public string? ReportCode { get; set; }

        public int PatientId { get; set; }
        public int LabTestId { get; set; }
        public int DoctorId { get; set; }
        public int LabTechnicianId { get; set; }

        [Required]
        public DateTime CollectionDate { get; set; }

        public DateTime? ReportDate { get; set; }

        [StringLength(100)]
        public string? ResultValue { get; set; }

        [StringLength(50)]
        public string? ResultUnit { get; set; }

        [StringLength(200)]
        public string? NormalRange { get; set; }

        public string? Findings { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Additional properties for display
        public string? PatientName { get; set; }
        public string? TestName { get; set; }
        public string? DoctorName { get; set; }
        public string? LabTechnicianName { get; set; }
        
        // Added property for Result
        public string Result { get; set; } = string.Empty;
    }
}