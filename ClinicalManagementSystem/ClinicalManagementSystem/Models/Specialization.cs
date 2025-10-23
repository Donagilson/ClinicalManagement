using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }

        [Required(ErrorMessage = "Specialization name is required")]
        [Display(Name = "Specialization Name")]
        public string SpecializationName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Department? Department { get; set; }
    }
}
