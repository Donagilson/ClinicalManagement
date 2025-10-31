using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IMedicineRepository
    {
        Task<int> AddMedicineAsync(Medicine medicine);
        Task<IEnumerable<Medicine>> GetAllMedicinesAsync();
        Task<Medicine?> GetMedicineByIdAsync(int id);
        Task<Medicine?> UpdateMedicineAsync(Medicine medicine);
        Task<bool> DeleteMedicineAsync(int id);
        Task<IEnumerable<Medicine>> GetMedicinesByCategoryAsync(string category);
        Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
        Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync();
        Task<Medicine?> GetMedicineByCodeAsync(string medicineCode);
        Task<bool> MedicineExistsAsync(string medicineCode, string medicineName);
        Task UpdateStockAsync(int medicineId, int quantity);
    }
}