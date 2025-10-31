using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    // Pharmacy-specific models (Medicine class moved to Medicine.cs to avoid conflicts)

    public class PharmacyInventory
    {
        public int InventoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MedicineId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CurrentStock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxStockLevel { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string BatchNumber { get; set; }

        [StringLength(100)]
        public string Supplier { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual Medicine Medicine { get; set; }
        public virtual ICollection<DispensedMedication> DispensedItems { get; set; }
    }

    public class DispensedMedication
    {
        public int DispensedId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int InventoryId { get; set; }

        public int? PrescriptionId { get; set; }

        public int? DoctorId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string Instructions { get; set; }

        [StringLength(200)]
        public string Dosage { get; set; }

        [StringLength(100)]
        public string Frequency { get; set; }

        [StringLength(100)]
        public string Duration { get; set; }

        [Required]
        public int DispensedBy { get; set; }

        [Required]
        public DateTime DispensedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Paid";

        [StringLength(100)]
        public string PaymentMethod { get; set; }

        public string Notes { get; set; }

        // Navigation properties
        public virtual Medicine Medicine { get; set; }
        public virtual PharmacyInventory InventoryItem { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Prescription Prescription { get; set; }
    }

    public class PharmacyReport
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineCode { get; set; }
        public int TotalDispensed { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CurrentStock { get; set; }
        public int LowStockCount { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
