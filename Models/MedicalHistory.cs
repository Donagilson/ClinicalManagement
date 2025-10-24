using System;

namespace ClinicalManagementSystem2025.Models
{
    public class MedicalHistory
    {
        public int MedicalHistoryId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string DoctorName { get; set; }
        public string RecordType { get; set; } // Prescription, Lab Report, Appointment
        public string Status { get; set; }
    }
}