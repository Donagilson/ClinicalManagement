using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ClinicalManagementSystem2025.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        public DateTime PrescriptionDate { get; set; } = DateTime.Now;

        public string Instructions { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public virtual List<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
    }
}