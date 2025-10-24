using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string Reason { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}