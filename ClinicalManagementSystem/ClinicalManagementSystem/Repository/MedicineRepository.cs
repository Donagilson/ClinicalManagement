using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public class MedicineRepository : IMedicineRepository
    {
        private static List<Medicine> _medicines = new List<Medicine>();
        private static int _nextMedicineId = 1;

        public MedicineRepository()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            if (!_medicines.Any())
            {
                _medicines.AddRange(new[]
                {
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED001",
                        MedicineName = "Paracetamol",
                        GenericName = "Acetaminophen",
                        Manufacturer = "Pharma Corp",
                        MedicineType = "Tablet",
                        DosageForm = "Oral",
                        Strength = "500mg",
                        Price = 5.00m,
                        StockQuantity = 100,
                        MinimumStockLevel = 10,
                        ExpiryDate = DateTime.Now.AddYears(2)
                    },
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED002",
                        MedicineName = "Amoxicillin",
                        GenericName = "Amoxicillin",
                        Manufacturer = "Medi Labs",
                        MedicineType = "Capsule",
                        DosageForm = "Oral",
                        Strength = "250mg",
                        Price = 15.00m,
                        StockQuantity = 50,
                        MinimumStockLevel = 10,
                        ExpiryDate = DateTime.Now.AddYears(1)
                    },
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED003",
                        MedicineName = "Ibuprofen",
                        GenericName = "Ibuprofen",
                        Manufacturer = "Health Pharma",
                        MedicineType = "Tablet",
                        DosageForm = "Oral",
                        Strength = "400mg",
                        Price = 8.00m,
                        StockQuantity = 5, // Low stock
                        MinimumStockLevel = 10,
                        ExpiryDate = DateTime.Now.AddYears(3)
                    }
                });
            }
        }

        public Task<Medicine?> GetMedicineByIdAsync(int id)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == id && m.IsActive);
            return Task.FromResult(medicine);
        }

        public Task<Medicine?> GetMedicineByCodeAsync(string medicineCode)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineCode == medicineCode && m.IsActive);
            return Task.FromResult(medicine);
        }

        public Task<IEnumerable<Medicine>> GetAllMedicinesAsync()
        {
            return Task.FromResult(_medicines.Where(m => m.IsActive).AsEnumerable());
        }

        public Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
        {
            var medicines = _medicines.Where(m => m.IsActive && (
                m.MedicineName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.GenericName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.MedicineCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ));
            return Task.FromResult(medicines.AsEnumerable());
        }

        public Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync()
        {
            var medicines = _medicines.Where(m => m.IsActive && m.StockQuantity <= m.MinimumStockLevel);
            return Task.FromResult(medicines.AsEnumerable());
        }

        public Task<Medicine> AddMedicineAsync(Medicine medicine)
        {
            medicine.MedicineId = _nextMedicineId++;
            medicine.CreatedDate = DateTime.Now;
            medicine.IsActive = true;
            _medicines.Add(medicine);
            return Task.FromResult(medicine);
        }

        public Task<Medicine?> UpdateMedicineAsync(Medicine medicine)
        {
            var existingMedicine = _medicines.FirstOrDefault(m => m.MedicineId == medicine.MedicineId);
            if (existingMedicine != null)
            {
                existingMedicine.MedicineCode = medicine.MedicineCode;
                existingMedicine.MedicineName = medicine.MedicineName;
                existingMedicine.GenericName = medicine.GenericName;
                existingMedicine.Manufacturer = medicine.Manufacturer;
                existingMedicine.MedicineType = medicine.MedicineType;
                existingMedicine.DosageForm = medicine.DosageForm;
                existingMedicine.Strength = medicine.Strength;
                existingMedicine.Description = medicine.Description;
                existingMedicine.StockQuantity = medicine.StockQuantity;
                existingMedicine.UnitPrice = medicine.UnitPrice;
                existingMedicine.ReorderLevel = medicine.ReorderLevel;
                existingMedicine.ExpiryDate = medicine.ExpiryDate;
                existingMedicine.UpdatedDate = DateTime.Now;
                existingMedicine.UpdatedBy = medicine.UpdatedBy;
            }
            return Task.FromResult(existingMedicine);
        }

        public Task<bool> DeleteMedicineAsync(int id)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == id);
            if (medicine != null)
            {
                medicine.IsActive = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> MedicineExistsAsync(string medicineCode, string medicineName)
        {
            return Task.FromResult(_medicines.Any(m =>
                (m.MedicineCode == medicineCode || m.MedicineName == medicineName) && m.IsActive));
        }

        public Task UpdateStockAsync(int medicineId, int quantity)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == medicineId);
            if (medicine != null)
            {
                medicine.StockQuantity += quantity;
            }
            return Task.CompletedTask;
        }
    }
}