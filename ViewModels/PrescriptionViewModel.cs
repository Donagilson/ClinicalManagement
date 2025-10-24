using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class PrescriptionViewModel
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Diagnosis is required")]
        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters")]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }

        [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
        [Display(Name = "Instructions")]
        public string Instructions { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "At least one medicine is required")]
        [MinLength(1, ErrorMessage = "Please add at least one medicine")]
        [Display(Name = "Medicines")]
        public List<MedicineItem> Medicines { get; set; } = new List<MedicineItem>();
    }

    public class MedicineItem
    {
        [Required(ErrorMessage = "Medicine selection is required")]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }

        public string MedicineName { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(50, ErrorMessage = "Dosage cannot exceed 50 characters")]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required")]
        [StringLength(50, ErrorMessage = "Frequency cannot exceed 50 characters")]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [StringLength(50, ErrorMessage = "Duration cannot exceed 50 characters")]
        [Display(Name = "Duration")]
        public string Duration { get; set; }

        [StringLength(200, ErrorMessage = "Instructions cannot exceed 200 characters")]
        [Display(Name = "Special Instructions")]
        public string Instructions { get; set; }
    }
}