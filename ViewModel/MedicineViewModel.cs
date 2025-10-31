using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModel
{
    public class MedicineViewModel
    {
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Medicine code is required")]
        [StringLength(20, ErrorMessage = "Medicine code cannot exceed 20 characters")]
        [Display(Name = "Medicine Code")]
        public string MedicineCode { get; set; }

        [Required(ErrorMessage = "Medicine name is required")]
        [StringLength(100, ErrorMessage = "Medicine name cannot exceed 100 characters")]
        [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; }

        [StringLength(100, ErrorMessage = "Generic name cannot exceed 100 characters")]
        [Display(Name = "Generic Name")]
        public string GenericName { get; set; }

        [StringLength(100, ErrorMessage = "Manufacturer cannot exceed 100 characters")]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [StringLength(50, ErrorMessage = "Medicine type cannot exceed 50 characters")]
        [Display(Name = "Medicine Type")]
        public string MedicineType { get; set; }

        [StringLength(50, ErrorMessage = "Dosage form cannot exceed 50 characters")]
        [Display(Name = "Dosage Form")]
        public string DosageForm { get; set; }

        [StringLength(50, ErrorMessage = "Strength cannot exceed 50 characters")]
        [Display(Name = "Strength")]
        public string Strength { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; } = 0;

        [Required(ErrorMessage = "Minimum stock level is required")]
        [Range(1, 1000, ErrorMessage = "Minimum stock level must be between 1 and 1,000")]
        [Display(Name = "Minimum Stock Level")]
        public int MinimumStockLevel { get; set; } = 10;

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Display properties
        public bool IsLowStock => StockQuantity <= MinimumStockLevel;
        public string StockStatus
        {
            get
            {
                if (StockQuantity == 0) return "Out of Stock";
                if (IsLowStock) return "Low Stock";
                return "In Stock";
            }
        }
        public string StockStatusClass
        {
            get
            {
                if (StockQuantity == 0) return "text-danger";
                if (IsLowStock) return "text-warning";
                return "text-success";
            }
        }

        // Validation methods
        public bool IsExpired => ExpiryDate != null && ExpiryDate < DateTime.Today;
        public bool WillExpireSoon => ExpiryDate != null &&
                                     ExpiryDate >= DateTime.Today &&
                                     ExpiryDate <= DateTime.Today.AddDays(30);
    }
}