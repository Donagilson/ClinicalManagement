using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModel
{
    public class LabReportViewModel
    {
        public int LabReportId { get; set; }

        [Required(ErrorMessage = "Report code is required")]
        [StringLength(20, ErrorMessage = "Report code cannot exceed 20 characters")]
        [Display(Name = "Report Code")]
        public string ReportCode { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Lab test is required")]
        [Display(Name = "Lab Test")]
        public int LabTestId { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        [Display(Name = "Referring Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Lab technician is required")]
        [Display(Name = "Lab Technician")]
        public int LabTechnicianId { get; set; }

        [Required(ErrorMessage = "Collection date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Collection Date")]
        public DateTime CollectionDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Report Date")]
        public DateTime? ReportDate { get; set; }

        [StringLength(100, ErrorMessage = "Result value cannot exceed 100 characters")]
        [Display(Name = "Result Value")]
        public string ResultValue { get; set; }

        [StringLength(50, ErrorMessage = "Result unit cannot exceed 50 characters")]
        [Display(Name = "Unit")]
        public string ResultUnit { get; set; }

        [StringLength(200, ErrorMessage = "Normal range cannot exceed 200 characters")]
        [Display(Name = "Normal Range")]
        public string NormalRange { get; set; }

        [Display(Name = "Findings")]
        public string Findings { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // Display properties
        public string PatientName { get; set; }
        public string TestName { get; set; }
        public string DoctorName { get; set; }
        public string LabTechnicianName { get; set; }
        public decimal TestPrice { get; set; }
        public string TestDescription { get; set; }

        // Validation properties
        public bool IsCompleted => Status == "Completed";
        public bool IsPending => Status == "Pending";
        public bool IsWithinNormalRange
        {
            get
            {
                if (string.IsNullOrEmpty(ResultValue) || string.IsNullOrEmpty(NormalRange))
                    return false;

                // Simple validation - in real application, implement proper range checking
                return true;
            }
        }
    }
}