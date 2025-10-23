using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class PrescriptionViewModel
    {
        public int PrescriptionId { get; set; }

        [Required(ErrorMessage = "Prescription code is required")]
        [StringLength(20, ErrorMessage = "Prescription code cannot exceed 20 characters")]
        [Display(Name = "Prescription Code")]
        public string PrescriptionCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Patient is required")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Display(Name = "Appointment")]
        public int? AppointmentId { get; set; }

        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters")]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Symptoms cannot exceed 500 characters")]
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; } = string.Empty;

        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Next Visit Date")]
        public DateTime? NextVisitDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        [DataType(DataType.DateTime)]
        [Display(Name = "Prescription Date")]
        public DateTime PrescriptionDate { get; set; } = DateTime.Now;

        // Display properties
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string AppointmentCode { get; set; } = string.Empty;

        // Prescription details
        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; } = new List<PrescriptionDetailViewModel>();
    }

    public class PrescriptionDetailViewModel
    {
        public int PrescriptionDetailId { get; set; }

        [Required(ErrorMessage = "Medicine is required")]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters")]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequency is required")]
        [StringLength(100, ErrorMessage = "Frequency cannot exceed 100 characters")]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [StringLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
        [Display(Name = "Duration")]
        public string Duration { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
        [Display(Name = "Instructions")]
        public string Instructions { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        // Display properties
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}