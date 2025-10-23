using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IMedicineRepository
    {
        Task<Medicine?> GetMedicineByIdAsync(int id);
        Task<Medicine?> GetMedicineByCodeAsync(string medicineCode);
        Task<IEnumerable<Medicine>> GetAllMedicinesAsync();
        Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
        Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync();
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine?> UpdateMedicineAsync(Medicine medicine);
        Task<bool> DeleteMedicineAsync(int id);
        Task<bool> MedicineExistsAsync(string medicineCode, string medicineName);
        Task UpdateStockAsync(int medicineId, int quantity);
    }
}