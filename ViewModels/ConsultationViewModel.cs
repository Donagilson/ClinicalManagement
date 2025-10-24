using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class ConsultationViewModel
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Symptoms are required")]
        [StringLength(1000, ErrorMessage = "Symptoms cannot exceed 1000 characters")]
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Diagnosis is required")]
        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters")]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }

        [StringLength(1000, ErrorMessage = "Treatment plan cannot exceed 1000 characters")]
        [Display(Name = "Treatment Plan")]
        public string TreatmentPlan { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }
    }
}