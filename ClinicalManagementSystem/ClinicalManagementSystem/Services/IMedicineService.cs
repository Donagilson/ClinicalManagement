using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Services
{
    public interface IMedicineService
    {
        Task<Medicine?> GetMedicineByIdAsync(int id);
        Task<Medicine?> GetMedicineByCodeAsync(string medicineCode);
        Task<IEnumerable<Medicine>> GetAllMedicinesAsync();
        Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
        Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync();
        Task<Medicine> CreateMedicineAsync(Medicine medicine);
        Task<Medicine?> UpdateMedicineAsync(int id, Medicine medicine);
        Task<bool> DeleteMedicineAsync(int id);
        Task UpdateStockAsync(int medicineId, int quantity);
    }
}