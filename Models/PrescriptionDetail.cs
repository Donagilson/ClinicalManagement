using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class PrescriptionDetail
    {
        [Key]
        public int PrescriptionDetailId { get; set; }

        public int PrescriptionId { get; set; }

        [ForeignKey("PrescriptionId")]
        public Prescription Prescription { get; set; }

        public int MedicineId { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Required]
        public string Duration { get; set; }

        public string Instructions { get; set; }
    }
}