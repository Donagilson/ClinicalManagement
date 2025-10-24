using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int? Experience { get; set; }
        public int DepartmentId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ConsultationFee { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}