using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class LabRequestViewModel
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Test name is required")]
        [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters")]
        [Display(Name = "Test Name")]
        public string TestName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Test Description")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }
    }
}