using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;

namespace ClinicalManagementSystem2025.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public Task<Medicine?> GetMedicineByIdAsync(int id)
        {
            return _medicineRepository.GetMedicineByIdAsync(id);
        }

        public Task<Medicine?> GetMedicineByCodeAsync(string medicineCode)
        {
            return _medicineRepository.GetMedicineByCodeAsync(medicineCode);
        }

        public Task<IEnumerable<Medicine>> GetAllMedicinesAsync()
        {
            return _medicineRepository.GetAllMedicinesAsync();
        }

        public Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
        {
            return _medicineRepository.SearchMedicinesAsync(searchTerm);
        }

        public Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync()
        {
            return _medicineRepository.GetLowStockMedicinesAsync();
        }

        public Task<Medicine> CreateMedicineAsync(Medicine medicine)
        {
            return _medicineRepository.AddMedicineAsync(medicine);
        }

        public async Task<Medicine?> UpdateMedicineAsync(int id, Medicine medicine)
        {
            var existingMedicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (existingMedicine == null)
            {
                return null;
            }

            medicine.MedicineId = id;
            return await _medicineRepository.UpdateMedicineAsync(medicine);
        }

        public Task<bool> DeleteMedicineAsync(int id)
        {
            return _medicineRepository.DeleteMedicineAsync(id);
        }

        public Task UpdateStockAsync(int medicineId, int quantity)
        {
            return _medicineRepository.UpdateStockAsync(medicineId, quantity);
        }
    }
}