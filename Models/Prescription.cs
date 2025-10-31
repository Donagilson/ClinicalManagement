// Models/Prescription.cs
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem2025.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public string? Diagnosis { get; set; }

        public DateTime PrescriptionDate { get; set; } = DateTime.Now;

        public string? Status { get; set; } = "Pending"; // Pending, Fulfilled, Cancelled

        public DateTime? FulfilledDate { get; set; }

        public string? Notes { get; set; }

        // Navigation properties
        public virtual Appointment? Appointment { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }
}