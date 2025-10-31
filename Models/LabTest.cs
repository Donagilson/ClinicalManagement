using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class LabTest
    {
        [Key]
        public int LabTestId { get; set; }

        [Required]
        [StringLength(20)]
        public string? TestCode { get; set; }

        [Required]
        [StringLength(100)]
        public string? TestName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string? NormalRange { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Additional properties for display
        public string? DepartmentName { get; set; }
    }
}