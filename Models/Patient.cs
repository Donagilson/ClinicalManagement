using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContact { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Add computed properties
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
}