using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicalManagementSystem.Models
{
    public class ConsultationViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }

        [Required]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public List<PrescribedMedicine> Medicines { get; set; } = new List<PrescribedMedicine>();
        public List<LabTestRequest> LabTests { get; set; } = new List<LabTestRequest>();
    }

    public class PrescribedMedicine
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Required]
        public string Duration { get; set; }

        [Display(Name = "Instructions")]
        public string Instructions { get; set; }
    }

    public class LabTestRequest
    {
        public string TestName { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
    }
}