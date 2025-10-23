using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class PrescriptionDetail
    {
        [Key]
        public int PrescriptionDetailId { get; set; }

        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Dosage { get; set; }

        [Required]
        [StringLength(100)]
        public string? Frequency { get; set; }

        [Required]
        [StringLength(100)]
        public string? Duration { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Additional properties for display
        public string? MedicineName { get; set; }
        public decimal Price { get; set; }
    }
}