using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "Doctor User")]
        public int UserId { get; set; }

        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Specialization")]
        public int? SpecializationId { get; set; }

        [Display(Name = "Qualification")]
        public string? Qualification { get; set; }

        [Display(Name = "Experience (Years)")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public int? Experience { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Consultation Fee")]
        [Range(0, 99999.99, ErrorMessage = "Consultation fee must be a positive value")]
        public decimal? ConsultationFee { get; set; }

        [Display(Name = "Available From")]
        public TimeSpan? AvailableFrom { get; set; }

        [Display(Name = "Available To")]
        public TimeSpan? AvailableTo { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public User? User { get; set; }
        public Department? Department { get; set; }
        public Specialization? SpecializationNavigation { get; set; }

        // Display property for backward compatibility
        public string DoctorName { get; set; } = string.Empty;

        // Full name property from User navigation
        public string FullName => User?.FullName ?? DoctorName;
    }
}
