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

        [StringLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        [StringLength(50)]
        public string MedicineType { get; set; } = string.Empty;

        [StringLength(50)]
        public string DosageForm { get; set; } = string.Empty;

        [StringLength(50)]
        public string Strength { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        public int MinimumStockLevel { get; set; } = 10;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int ReorderLevel { get; set; } = 5;

        // Added for backward compatibility with existing code
        public decimal Price {
            get { return UnitPrice; }
            set { UnitPrice = value; }
        }

        // Additional property for display
        public bool IsLowStock => StockQuantity <= MinimumStockLevel;

        // NEW PHARMACY PROPERTIES
        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(50)]
        public string? Form { get; set; }

        public bool RequiresPrescription { get; set; } = false;

        // Navigation properties for pharmacy module
        public virtual ICollection<PharmacyInventory>? InventoryItems { get; set; }
        public virtual ICollection<DispensedMedication>? DispensedItems { get; set; }
        public virtual ICollection<PrescriptionDetail>? PrescriptionDetails { get; set; }
    }
}