using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalManagementSystem2025.Models
{
    public class ConsultationNotes
    {
        [Key]
        public int ConsultationId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public string Symptoms { get; set; }

        public string Diagnosis { get; set; }

        public string TreatmentPlan { get; set; }

        public string Notes { get; set; }

        public DateTime ConsultationDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}