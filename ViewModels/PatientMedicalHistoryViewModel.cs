using ClinicalManagementSystem2025.Models;
using System;
using System.Collections.Generic;

namespace ClinicalManagementSystem2025.ViewModels
{
    public class PatientMedicalHistoryViewModel
    {
        public Patient Patient { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<LabReport> LabReports { get; set; }
        public List<ConsultationNotes> ConsultationNotes { get; set; }
        public DateTime? LastVisit { get; set; }

        // Helper properties
        public int TotalPrescriptions => Prescriptions?.Count ?? 0;
        public int TotalLabReports => LabReports?.Count ?? 0;
        public int TotalConsultations => ConsultationNotes?.Count ?? 0;

        public string LastVisitDisplay => LastVisit?.ToString("MMM dd, yyyy") ?? "No visits";
    }
}