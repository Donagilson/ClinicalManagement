using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class LabReport
    {
        [Key]
        public int LabReportId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string TestName { get; set; }
        public DateTime TestDate { get; set; }
        public string Result { get; set; }
        public int? TechnicianId { get; set; }
        public string Status { get; set; } = "Pending";
        public string Notes { get; set; }

        // Add these navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}