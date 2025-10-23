using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        [Required]
        [StringLength(20)]
        public string MedicineCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string GenericName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MedicineType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DosageForm { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Strength { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; } = 0;

        public int MinimumStockLevel { get; set; } = 10;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
        
        // Added for backward compatibility with existing code
        public decimal Price { 
            get { return UnitPrice; } 
            set { UnitPrice = value; } 
        }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int ReorderLevel { get; set; } = 5;

        // Additional property for display
        public bool IsLowStock => StockQuantity <= MinimumStockLevel;
    }
}