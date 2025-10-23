using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Patient code is required")]
        [StringLength(20, ErrorMessage = "Patient code cannot exceed 20 characters")]
        [Display(Name = "Patient Code")]
        public string PatientCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Emergency contact name cannot exceed 100 characters")]
        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [StringLength(5, ErrorMessage = "Blood group cannot exceed 5 characters")]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Allergies cannot exceed 500 characters")]
        [Display(Name = "Allergies")]
        public string Allergies { get; set; } = string.Empty;

        [Display(Name = "Medical History")]
        public string MedicalHistory { get; set; } = string.Empty;

        // Display properties
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}