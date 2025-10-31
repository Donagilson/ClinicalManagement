using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IMedicalNoteRepository
    {
        Task<int> AddMedicalNoteAsync(MedicalNote medicalNote);
        Task<IEnumerable<MedicalNote>> GetMedicalNotesByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalNote>> GetMedicalNotesByDoctorIdAsync(int doctorId);
        Task<MedicalNote?> GetMedicalNoteByIdAsync(int medicalNoteId);
        Task<bool> UpdateMedicalNoteAsync(MedicalNote medicalNote);
        Task<bool> DeleteMedicalNoteAsync(int medicalNoteId);
    }
}
