using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class PharmacyPrescription
    {
        [Key]
        public int PharmacyPrescriptionId { get; set; }

        public int PrescriptionId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? DispensedDate { get; set; }
        public int? PharmacistId { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        //[ForeignKey("PharmacistId")]
        //public virtual User Pharmacist { get; set; }
    }

    public class PharmacyPrescriptionViewModel
    {
        public int PharmacyPrescriptionId { get; set; }
        public int PrescriptionId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public string DoctorName { get; set; }
        public string Diagnosis { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? DispensedDate { get; set; }
        public string Notes { get; set; }
        public List<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
    }
}