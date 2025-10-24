using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}